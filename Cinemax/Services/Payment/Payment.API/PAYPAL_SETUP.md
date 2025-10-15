# PayPal Developer Account Setup Guide

This guide will help you set up PayPal integration with your developer account.

## 1. Create PayPal Developer Account

1. Go to [PayPal Developer Portal](https://developer.paypal.com/)
2. Sign in with your PayPal account or create a new one
3. Navigate to "My Apps & Credentials"

## 2. Create a New Application

1. Click "Create App"
2. Fill in the application details:
   - **App Name**: Cinemax Payment API (or your preferred name)
   - **Merchant**: Select your business account
   - **Features**: Select "Accept payments"
3. Click "Create App"

## 3. Get Your Credentials

After creating the app, you'll see:
- **Client ID**: Copy this value
- **Client Secret**: Copy this value (click "Show" to reveal)

## 4. Configure Your Application

### For Sandbox Environment (Testing):
1. Open `Payment.API/appsettings.Development.json`
2. Replace the placeholder values:
   ```json
   "PayPal": {
     "ApiUrl": "https://api-m.sandbox.paypal.com",
     "ClientId": "YOUR_ACTUAL_CLIENT_ID_HERE",
     "ClientSecret": "YOUR_ACTUAL_CLIENT_SECRET_HERE"
   }
   ```

### For Production Environment:
1. Open `Payment.API/appsettings.json`
2. Add the production configuration:
   ```json
   "PayPal": {
     "ApiUrl": "https://api-m.paypal.com",
     "ClientId": "YOUR_PRODUCTION_CLIENT_ID_HERE",
     "ClientSecret": "YOUR_PRODUCTION_CLIENT_SECRET_HERE"
   }
   ```

## 5. Test Account Setup

### Create Test Accounts:
1. In PayPal Developer Portal, go to "Sandbox" → "Accounts"
2. Create test accounts:
   - **Personal Account**: For testing buyer experience
   - **Business Account**: For testing merchant experience

### Test Credentials:
- Use the test account credentials when testing the payment flow
- The system will use your actual PayPal app credentials for API calls

## 6. Webhook Configuration (Optional)

For production, you may want to set up webhooks:
1. In your PayPal app settings, go to "Webhooks"
2. Add webhook URL: `https://yourdomain.com/api/paypal/webhook`
3. Select events: `PAYMENT.SALE.COMPLETED`, `PAYMENT.SALE.DENIED`

## 7. Testing the Integration

1. Start your Payment API: `docker compose up -d payment.api`
2. Go to `http://localhost:4200/paypal`
3. Create a payment with amount $0.10
4. Click "Approve Payment on PayPal Sandbox"
5. Use your test account credentials to approve the payment

## 8. Environment Variables (Alternative)

Instead of appsettings, you can use environment variables:
```bash
export PayPal__ClientId="your_client_id"
export PayPal__ClientSecret="your_client_secret"
export PayPal__ApiUrl="https://api-m.sandbox.paypal.com"
```

## 9. Security Best Practices

1. **Never commit credentials to version control**
2. **Use environment variables in production**
3. **Rotate credentials regularly**
4. **Use HTTPS in production**
5. **Validate webhook signatures**

## 10. Troubleshooting

### Common Issues:
- **Invalid credentials**: Double-check Client ID and Secret
- **CORS errors**: Ensure your domain is whitelisted in PayPal app settings
- **Payment not approved**: Use correct test account credentials
- **API errors**: Check PayPal API status and your app permissions

### Debug Mode:
Enable detailed logging by setting log level to "Debug" in appsettings.

## 11. Production Checklist

Before going live:
- [ ] Switch to production PayPal API URL
- [ ] Use production credentials
- [ ] Test with real PayPal accounts
- [ ] Set up webhook endpoints
- [ ] Configure proper error handling
- [ ] Set up monitoring and logging
- [ ] Review PayPal's terms of service

## Support

- [PayPal Developer Documentation](https://developer.paypal.com/docs/)
- [PayPal API Reference](https://developer.paypal.com/docs/api/)
- [PayPal Support](https://www.paypal.com/support/)
