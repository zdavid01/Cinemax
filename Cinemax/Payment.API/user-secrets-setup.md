# User Secrets Setup for Payment API

To set up user secrets for the Payment API, run the following commands in the Payment.API directory:

## 1. Initialize User Secrets
```bash
dotnet user-secrets init
```

## 2. Add Sensitive Configuration
```bash
# Email Settings
dotnet user-secrets set "EmailSettings:Mail" "vida33085@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "kmfu haci tnno kjqd"

# PayPal Settings
dotnet user-secrets set "PayPal:ClientId" "AbwOUURIy30Sv997qpH0-Xzwb-rKENp64r2-F8jJx-GNMHcQ9ZFIwvQLjP2-sMe7-u-kg2AGAjjKtGSk"
dotnet user-secrets set "PayPal:ClientSecret" "EP--0kzPAWPT9yg5or-0qKVMWnjusLoqZ-aSIBWhtMouaWcbyORFvfXPPKKCNU77rzq4LHA_d_D9OvI7"

# JWT Settings
dotnet user-secrets set "JwtSettings:secretKey" "MyVerySecretMessageWhichIsVeryVeryLongAndCannotBeBroken"
```

## 3. Verify Secrets
```bash
dotnet user-secrets list
```

## 4. For Docker Environment
The Docker environment will use the secrets from the appsettings.Development.json file, which now only contains non-sensitive configuration.

## Benefits
- Sensitive data is stored locally and not committed to version control
- Each developer can have their own secrets
- Production deployments can use environment variables or Azure Key Vault
- Better security posture for the application
