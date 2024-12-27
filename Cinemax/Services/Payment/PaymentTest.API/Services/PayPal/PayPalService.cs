using PaymentTest.API.Services.Email.Services;
using PaymentTest.API.Services.PayPal;

namespace Paypal;

using RestSharp;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

public class PayPalService
{
    private string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
    private string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
    private const string PayPalApiUrl = "https://api-m.sandbox.paypal.com";  // Change to live URL for production

    private RestClient client;
    private string accessToken;

    //TODO dependency injection
    private readonly GmailService gmailService;
    
    public PayPalService()
    {
        client = new RestClient(PayPalApiUrl);
    }

    // Step 1: Get an OAuth token from PayPal
    public async Task<dynamic> GetAccessToken()
    {
        var request = new RestRequest("v1/oauth2/token", Method.Post);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
        request.AddParameter("grant_type", "client_credentials");
        request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            accessToken = jsonResponse.access_token;
            return accessToken;
        }
        else
        {
            throw new Exception($"Error getting access token: {response.Content}");
        }
    }

    // Step 2: Create a payment
    public async Task<CreatePayPalPaymentResponse> CreatePayment(decimal amount, string currency = "USD", 
        string returnUrl = "http://localhost:5281/api/paypal/return", string cancelUrl = "http://localhost:5017/swagger/index.html/api/paypal/cancel")
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            await GetAccessToken();
        }

        var request = new RestRequest("v1/payments/payment", Method.Post);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
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
                return_url = returnUrl,
                cancel_url = cancelUrl
            }
        };

        request.AddJsonBody(paymentData);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string? approvalUrl = "";
            foreach (var link in jsonResponse?.links)
            {
                if (link.rel == "approval_url")
                {
                    string paymentId = jsonResponse.id;
                    approvalUrl = link.href;
                    return new CreatePayPalPaymentResponse { PaymentId = paymentId, ApprovalUrl = approvalUrl};
                    return link.href; // This is the approval URL
                }
            }
        }
        
        else
        {
            throw new Exception($"Error creating payment: {response.Content}");
        }
        return null;
    }

    // Step 3: Execute a payment after approval
    public async Task<string> ExecutePayment(string paymentId, string payerId)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            await GetAccessToken();
        }

        var request = new RestRequest($"v1/payments/payment/{paymentId}/execute", Method.Post);
        request.AddHeader("Authorization", $"Bearer {accessToken}");
        request.AddHeader("Content-Type", "application/json");

        var paymentExecution = new
        {
            payer_id = payerId
        };

        request.AddJsonBody(paymentExecution);

        var response = await client.ExecuteAsync(request);

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
