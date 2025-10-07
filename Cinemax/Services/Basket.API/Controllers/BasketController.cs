using AutoMapper;
using Basket.API.Entities;
//using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using Grpc.Core;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

    public BasketController(IBasketRepository basketRepository, ILogger<BasketController> logger,
        IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        // _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
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
            return BadRequest();
        }

        // Check if payment was successful

		// Send checkout event
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        await _publishEndpoint.Publish(eventMessage);

        // Remove the basket
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