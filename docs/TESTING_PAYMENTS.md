# 💳 Testing Payment Functionality

## Overview

This guide explains how to test the PayPal payment integration in the Cinemax project using sandbox credentials.

---

## 🧪 PayPal Sandbox Setup

### What is PayPal Sandbox?

PayPal Sandbox is a testing environment that simulates real PayPal transactions without processing actual money. Perfect for development and testing!

### Included Credentials

The project includes working PayPal sandbox credentials in `.env.example`:

**API Credentials (for the application):**
```env
PAYPAL_CLIENT_ID=AbwOUURIy30Sv997qpH0-Xzwb-rKENp64r2-F8jJx-GNMHcQ9ZFIwvQLjP2-sMe7-u-kg2AGAjjKtGSk
PAYPAL_CLIENT_SECRET=EP--0kzPAWPT9yg5or-0qKVMWnjusLoqZ-aSIBWhtMouaWcbyORFvfXPPKKCNU77rzq4LHA_d_D9OvI7
```

**Test Buyer Account (for manual testing):**
```
Email: sb-2qjuu34887226@personal.example.com
Password: 4)grxJ35
```

---

## 🚀 Testing Payment Flow

### Step 1: Start the Services

```bash
cd Cinemax/Cinemax
cp .env.example .env
docker-compose up -d
```

### Step 2: Create a Test User

Use the `test-admin.http` file to register a buyer:

```http
POST http://localhost:4000/api/v1/Authentication/RegisterBuyer
Content-Type: application/json

{
  "firstName": "Test",
  "lastName": "Buyer",
  "username": "testbuyer",
  "password": "Test123!",
  "email": "test@example.com"
}
```

### Step 3: Login and Get Token

```http
POST http://localhost:4000/api/v1/Authentication/Login
Content-Type: application/json

{
  "username": "testbuyer",
  "password": "Test123!"
}
```

Copy the returned JWT token.

### Step 4: Add Movies to Basket

```http
POST http://localhost:8001/api/v1/Basket
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "username": "testbuyer",
  "items": [
    {
      "movieId": "602d2149e773f2a3990b47f5",
      "title": "Star Wars: The Force Awakens",
      "price": 9.99
    }
  ]
}
```

### Step 5: Initiate Checkout

```http
POST http://localhost:8001/api/v1/Basket/checkout
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

{
  "buyerId": "testbuyer-id",
  "buyerUsername": "testbuyer",
  "emailAddress": "test@example.com",
  "street": "123 Test St",
  "city": "Test City",
  "state": "TS",
  "country": "Test Country",
  "zipCode": "12345"
}
```

This will:
1. ✅ Call Payment.API via gRPC
2. ✅ Create payment record in database
3. ✅ Return PayPal payment URL

### Step 6: Complete Payment via PayPal

When you get the PayPal payment URL:

1. Open the URL in your browser
2. Log in with the PayPal sandbox account:
   - **Email:** `sb-2qjuu34887226@personal.example.com`
   - **Password:** `4)grxJ35`
3. Complete the mock payment
4. You'll be redirected back to the application

### Step 7: Verify Payment

Check the payment was recorded:

```http
GET http://localhost:8004/api/v1/Payment/user/testbuyer
Authorization: Bearer YOUR_TOKEN_HERE
```

---

## 🧪 Testing Different Scenarios

### Test Successful Payment

Use the sandbox buyer account provided above - it has unlimited funds in sandbox mode.

### Test Failed Payment

1. Use an invalid PayPal email
2. Cancel the payment on PayPal page
3. Verify error handling in your application

### Test Email Notifications

After successful payment:
1. Check Email.API logs: `docker-compose logs email.api`
2. Look for: "Email sent successfully for event..."
3. Emails are sent via RabbitMQ asynchronously

---

## 📊 Payment Service Architecture

```
Frontend/Basket.API
    ↓ (gRPC)
Payment.API
    ↓ (PayPal REST API)
PayPal Sandbox
    ↓ (Redirect)
User Completes Payment
    ↓ (Webhook/Return URL)
Payment.API
    ↓ (RabbitMQ)
Email.API → Send confirmation email
```

---

## 🔍 Monitoring Payments

### View Payment Logs

```bash
# Payment API logs
docker-compose logs -f payment.api

# Look for:
# - "Payment {id} created successfully"
# - PayPal API responses
# - Error messages
```

### Access PayPal Sandbox Dashboard

1. Go to: https://developer.paypal.com/dashboard/
2. Log in with your PayPal developer account
3. Navigate to "Sandbox" → "Accounts"
4. View test transactions

### Check Database

```bash
# Connect to PostgreSQL
docker exec -it cinemax-paymentdb-1 psql -U postgres -d PaymentDb

# View payments
SELECT * FROM "Payments";

# Exit
\q
```

---

## 💡 Tips

1. **Sandbox Mode**: All payments are simulated - no real money ever changes hands
2. **Test Account**: The provided PayPal account has unlimited test funds
3. **Email Confirmation**: Check Email.API logs to see confirmation emails being processed
4. **gRPC Communication**: Basket → Payment communication happens via gRPC (see logs)
5. **Async Processing**: Email sending is async via RabbitMQ

---

## 🐛 Troubleshooting

### Payment Fails with "Service Unavailable"

Check if Payment.API is running:
```bash
docker-compose ps payment.api
docker-compose logs payment.api
```

### Can't Log Into PayPal Sandbox

- Make sure you're using the **sandbox** credentials, not production
- Check if PayPal sandbox is accessible: https://www.sandbox.paypal.com
- Try clearing browser cookies/cache

### Email Not Sent

Check RabbitMQ connection:
```bash
docker-compose logs email.api | grep -i rabbit
docker-compose logs payment.api | grep -i "Bus started"
```

### gRPC Error from Basket

Verify Payment.API is running on port 8080:
```bash
curl http://localhost:8004/health
```

---

## 📚 Related Documentation

- **PayPal Setup:** `Cinemax/Services/Payment/Payment.API/PAYPAL_SETUP.md`
- **Security Guide:** `docs/SECURITY_GUIDE.md`
- **Development Credentials:** `docs/DEVELOPMENT_CREDENTIALS.md`

---

**Last Updated:** October 2025

