using Newtonsoft.Json;
using RestSharp;

namespace Payment.Infrastructure.PayPal;


public class PayPalService
{
    private const string PayPalApiUrl = "https://api-m.sandbox.paypal.com";
    private string _clientId = "AbwOUURIy30Sv997qpH0-Xzwb-rKENp64r2-F8jJx-GNMHcQ9ZFIwvQLjP2-sMe7-u-kg2AGAjjKtGSk";
    private string _clientSecret = "EP--0kzPAWPT9yg5or-0qKVMWnjusLoqZ-aSIBWhtMouaWcbyORFvfXPPKKCNU77rzq4LHA_d_D9OvI7";

    private RestClient _client;
    private string _accessToken;

    public PayPalService()
    {
        _client = new RestClient(PayPalApiUrl);
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


    public async Task<string> CreatePayment(decimal amount, string currency = "USD", 
        string? returnUrl = "http://localhost:5017/swagger/index.html/api/paypal/return", string? cancelUrl = "http://localhost:5017/swagger/index.html/api/paypal/cancel" )
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
            
            foreach (var link in jsonResponse.links)
            {
                if (link.rel == "approval_url")
                {
                    //TODO return link and payerId
                    return link.href;  // This is the approval URL
                }
            }
        }
        else
        {
            throw new Exception($"Error creating payment: {response.Content}");
        }
        
        //TODO return something meaningful ?
        return null;
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