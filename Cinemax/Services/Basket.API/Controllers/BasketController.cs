using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using Grpc.Core;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Payment.API.Protos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ShoppingCartItem = Basket.API.Entities.ShoppingCartItem;

[Authorize(Roles = "Buyer")]
[ApiController]
[Route("api/v1/[controller]")]
public class BasketController: ControllerBase
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<BasketController> _logger;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly PaymentGrpcClient _paymentGrpcClient;

    public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger,
        IMapper mapper, IPublishEndpoint publishEndpoint, PaymentGrpcClient paymentGrpcClient)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _paymentGrpcClient = paymentGrpcClient ?? throw new ArgumentNullException(nameof(paymentGrpcClient));
    }

    [HttpGet("{username}")]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string username)
    {
        if (User.FindFirst(ClaimTypes.Name).Value != username)
        {
            return Forbid();
        }
        
        var basket = await _basketRepository.GetBasket(username);
        if (basket == null)
            return NotFound();
        return Ok(basket ?? new ShoppingCart(username));
    }
    
    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        if (User.FindFirst(ClaimTypes.Name).Value != basketCheckout.BuyerUsername)
        {
            return Forbid();
        }
        
        // Get existing basket
        var basket = await _basketRepository.GetBasket(basketCheckout.BuyerUsername);
        if (basket == null)
        {
            return BadRequest("Basket not found");
        }

        // Initiate payment via gRPC
        try
        {
            var checkoutRequest = new CheckoutRequest
            {
                BuyerId = basketCheckout.BuyerId,
                BuyerUsername = basketCheckout.BuyerUsername,
                EmailAddress = basketCheckout.EmailAddress,
                TotalPrice = (double)basket.TotalPrice,
                Currency = "USD"
            };

            // Add items to the request
            foreach (var item in basket.Items)
            {
                checkoutRequest.Items.Add(new CheckoutItem
                {
                    MovieId = item.MovieId,
                    MovieName = item.Title,
                    Price = (double)item.Price,
                    Quantity = 1
                });
            }

            var paymentResponse = await _paymentGrpcClient.InitiateCheckout(checkoutRequest);

            if (!paymentResponse.Success)
            {
                _logger.LogWarning($"Payment initiation failed: {paymentResponse.Message}");
                return BadRequest($"Payment failed: {paymentResponse.Message}");
            }

            _logger.LogInformation($"Payment {paymentResponse.PaymentId} initiated successfully");
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "gRPC call to Payment service failed");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Payment service unavailable");
        }

		// Send checkout event
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        await _publishEndpoint.Publish(eventMessage);

        // Remove the basket after successful payment
        await _basketRepository.DeleteBasket(basketCheckout.BuyerUsername);

        return Accepted();
    }

    [HttpPut]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingCart>> AddOrUpdateBasket([FromBody] ShoppingCart basket)
    {
        if (User.FindFirst(ClaimTypes.Name).Value != basket.Username)
        {
            return Forbid();
        }

        if (basket == null || basket.Items == null)
        {
            return BadRequest();
        }

        var updatedBasket = await _basketRepository.UpdateBasket(basket);
        return Ok(updatedBasket);
    }

    [HttpGet("movies/{username}")]
    [ProducesResponseType(typeof(IEnumerable<ShoppingCartItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> GetMoviesFromCart(string username)
    {
        if (User.FindFirst(ClaimTypes.Name).Value != username)
        {
            return Forbid();
        }

        var basket = await _basketRepository.GetBasket(username);
        if (basket == null || basket.Items == null || !basket.Items.Any())
        {
            return NotFound();
        }

        return Ok(basket.Items);
    }

    [HttpDelete("{username}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBasket(string username)
    {
        if (User.FindFirst(ClaimTypes.Name).Value != username)
        {
            return Forbid();
        }
        
        await _basketRepository.DeleteBasket(username);
        return Ok();
    }
}