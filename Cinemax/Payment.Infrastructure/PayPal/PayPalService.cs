using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;

namespace Payment.Infrastructure.PayPal;


public class PayPalService
{
    private readonly string _payPalApiUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;

    private RestClient _client;
    private string _accessToken;

    public PayPalService(IConfiguration configuration)
    {
        _payPalApiUrl = configuration["PayPal:ApiUrl"] ?? "https://api-m.sandbox.paypal.com";
        _clientId = configuration["PayPal:ClientId"] ?? throw new ArgumentNullException("PayPal:ClientId is required");
        _clientSecret = configuration["PayPal:ClientSecret"] ?? throw new ArgumentNullException("PayPal:ClientSecret is required");
        
        _client = new RestClient(_payPalApiUrl);
    }

    //Get an OAuth token from PayPal
    public async Task<string> GetAccessToken()
    {
        var request = new RestRequest("v1/oauth2/token", Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes(string.Format($"{_clientId}:{_clientSecret}"))));
        
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            _accessToken = jsonResponse.access_token;
            return _accessToken;
        }
        else
        {
            throw new Exception($"Error getting access token: {response.ErrorMessage}");
        }
    }


    public async Task<(string paymentId, string approvalUrl)> CreatePayment(decimal amount, string currency = "USD", 
        string? returnUrl = "http://localhost:8004/api/paypal/return", string? cancelUrl = "http://localhost:8004/api/paypal/cancel" )
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            await GetAccessToken();
        }

        var request = new RestRequest("v1/payments/payment", Method.Post);
        request.AddHeader("Authorization", $"Bearer {_accessToken}");
        request.AddHeader("Content-Type", "application/json");

        
        var paymentData = new
        {
            intent = "sale",
            payer = new
            {
                payment_method = "paypal"
            },
            transactions = new[]
            {
                new
                {
                    amount = new
                    {
                        total = amount.ToString("F2"),
                        currency
                    },
                    description = "Payment transaction"
                }
            },
            redirect_urls = new
            {
                return_url = returnUrl, //TODO handle null
                cancel_url = cancelUrl //TODO handle null
            }
        };
        
        request.AddJsonBody(paymentData);
        
        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string paymentId = jsonResponse.id;
            string approvalUrl = "";
            
            foreach (var link in jsonResponse.links)
            {
                if (link.rel == "approval_url")
                {
                    approvalUrl = link.href;
                    break;
                }
            }
            
            return (paymentId, approvalUrl);
        }
        else
        {
            throw new Exception($"Error creating payment: {response.Content}");
        }
    }

    public async Task<string> ExecutePayment(string paymentId, string payerId)
    {
        if (string.IsNullOrEmpty(_accessToken))
        {
            await GetAccessToken();
        }

        var request = new RestRequest($"v1/payments/payment/{paymentId}/execute", Method.Post);
        request.AddHeader("Authorization", $"Bearer {_accessToken}");
        request.AddHeader("Content-Type", "application/json");

        var paymentExecution = new
        {
            payer_id = payerId
        };

        request.AddJsonBody(paymentExecution);

        var response = await _client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return jsonResponse.state; // "approved" if successful
        }
        else
        {
            throw new Exception($"Error executing payment: {response.Content}");
        }
    }
    
}