# Docker Secrets Setup for Payment API

## Current Setup
The Payment API now uses environment variables in docker-compose.override.yml for sensitive configuration.

## For Production/More Secure Setup

### Option 1: Use .env file (Recommended for Docker)
Create a `.env` file in the project root with:

```env
# Email Settings
EMAIL_MAIL=vida33085@gmail.com
EMAIL_PASSWORD=kmfu haci tnno kjqd

# PayPal Settings
PAYPAL_CLIENT_ID=AbwOUURIy30Sv997qpH0-Xzwb-rKENp64r2-F8jJx-GNMHcQ9ZFIwvQLjP2-sMe7-u-kg2AGAjjKtGSk
PAYPAL_CLIENT_SECRET=EP--0kzPAWPT9yg5or-0qKVMWnjusLoqZ-aSIBWhtMouaWcbyORFvfXPPKKCNU77rzq4LHA_d_D9OvI7

# JWT Settings
JWT_SECRET_KEY=MyVerySecretMessageWhichIsVeryVeryLongAndCannotBeBroken
```

Then update docker-compose.override.yml to use:
```yaml
payment.api:
  environment:
    - "EmailSettings:Mail=${EMAIL_MAIL}"
    - "EmailSettings:Password=${EMAIL_PASSWORD}"
    - "PayPal:ClientId=${PAYPAL_CLIENT_ID}"
    - "PayPal:ClientSecret=${PAYPAL_CLIENT_SECRET}"
    - "JwtSettings:secretKey=${JWT_SECRET_KEY}"
```

### Option 2: Use Docker Secrets
For even more security, use Docker secrets:

```yaml
payment.api:
  secrets:
    - email_password
    - paypal_client_secret
    - jwt_secret_key
  environment:
    - "EmailSettings:Password_FILE=/run/secrets/email_password"
    - "PayPal:ClientSecret_FILE=/run/secrets/paypal_client_secret"
    - "JwtSettings:secretKey_FILE=/run/secrets/jwt_secret_key"

secrets:
  email_password:
    file: ./secrets/email_password.txt
  paypal_client_secret:
    file: ./secrets/paypal_client_secret.txt
  jwt_secret_key:
    file: ./secrets/jwt_secret_key.txt
```

## Benefits of Current Setup
- Sensitive data is separated from code
- Easy to manage different environments
- Can be easily updated without rebuilding containers
- Follows security best practices
