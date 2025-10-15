# 🛡️ Security Guide for Cinemax

## Overview

This guide covers security best practices for the Cinemax project, including credential management, environment variables, and production deployment security.

## 🎯 Security Philosophy

**Development vs Production:**
- **Development**: `.env.example` contains **working sandbox/test credentials** that are safe to share with your team
- **Production**: You MUST use your own secure credentials and proper secret management

**Why this approach?**
- ✅ New developers can start immediately without hunting for credentials
- ✅ PayPal sandbox credentials don't process real transactions
- ✅ Development email is a shared team account
- ✅ Easy onboarding and testing
- ⚠️ Clear separation between dev and production

---

## 🔐 Credential Management

### Current Security Implementation

The project now uses **environment variables** to protect sensitive credentials:

- ✅ `.env` file for local development (gitignored)
- ✅ `.env.example` template for onboarding new developers
- ✅ Docker Compose reads from environment variables
- ✅ Default fallback values for quick testing

### Environment Variables Used

| Variable | Used By | Purpose |
|----------|---------|---------|
| `DB_SA_PASSWORD` | Identity.API | MS SQL Server password |
| `POSTGRES_PASSWORD` | Payment.API | PostgreSQL password |
| `PAYPAL_CLIENT_ID` | Payment.API | PayPal API client ID |
| `PAYPAL_CLIENT_SECRET` | Payment.API | PayPal API secret |
| `EMAIL_ADDRESS` | Payment.API, Email.API | Gmail address |
| `EMAIL_APP_PASSWORD` | Payment.API, Email.API | Gmail app password |
| `JWT_SECRET_KEY` | All APIs | JWT signing key |
| `GOOGLE_DRIVE_FOLDER_ID` | PrivateSession.API | Google Drive folder |
| `PGADMIN_EMAIL` | pgAdmin | Admin UI email |
| `PGADMIN_PASSWORD` | pgAdmin | Admin UI password |

---

## ⚠️ Security Risks & Mitigation

### High Risk Items

#### 1. **PayPal API Credentials**
- **Risk**: Financial fraud, unauthorized transactions
- **Mitigation**: 
  - Use sandbox credentials for development
  - Rotate production credentials monthly
  - Enable PayPal webhook notifications for suspicious activity
  - Never log API responses containing sensitive data

#### 2. **JWT Secret Key**
- **Risk**: Authentication bypass, token forgery
- **Mitigation**:
  - Minimum 256-bit secret (32+ characters)
  - Generate with: `openssl rand -base64 64`
  - Different secrets for each environment
  - Rotate if compromised

#### 3. **Email App Password**
- **Risk**: Spam, phishing, email account compromise
- **Mitigation**:
  - Use Gmail app passwords (not account password)
  - Enable 2FA on the email account
  - Monitor for suspicious activity
  - Consider using SendGrid/AWS SES for production

#### 4. **Google Service Account Key**
- **Risk**: Unauthorized access to Google Drive content
- **Current Setup**: 
  - Development service account (`cinemax-475115-7345803004e9.json`) is **included in repository**
  - This is intentional and safe because:
    - ✅ Restricted to a specific development folder
    - ✅ Folder contains only test videos
    - ✅ Can be rotated/revoked anytime
    - ✅ Not connected to personal accounts
- **Mitigation**:
  - Development: Use the included shared service account
  - Production: Create separate service account with production access
  - Store production credentials in secure vault (never commit)
  - Rotate keys annually or when team members leave

---

## 🔒 Production Deployment Security

### DO NOT Use .env in Production!

For production deployments, use proper secret management:

### Option 1: Docker Secrets (Docker Swarm)

```yaml
# docker-compose.prod.yml
services:
  payment.api:
    environment:
      - PayPal:ClientId=/run/secrets/paypal_client_id
      - PayPal:ClientSecret=/run/secrets/paypal_client_secret
    secrets:
      - paypal_client_id
      - paypal_client_secret

secrets:
  paypal_client_id:
    external: true
  paypal_client_secret:
    external: true
```

Create secrets:
```bash
echo "your_client_id" | docker secret create paypal_client_id -
echo "your_client_secret" | docker secret create paypal_client_secret -
```

### Option 2: Kubernetes Secrets

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: cinemax-secrets
type: Opaque
stringData:
  paypal-client-id: "your_client_id"
  paypal-client-secret: "your_client_secret"
  jwt-secret: "your_jwt_secret"
```

### Option 3: Cloud Secret Management

**Azure Key Vault:**
```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

**AWS Secrets Manager:**
```csharp
builder.Configuration.AddSecretsManager();
```

**Google Cloud Secret Manager:**
```csharp
builder.Configuration.AddGoogleSecretManager();
```

---

## 🚨 If Secrets Are Compromised

### Immediate Actions

1. **Rotate Compromised Credentials Immediately**
   - PayPal: Generate new API credentials in dashboard
   - Email: Revoke app password and generate new one
   - JWT: Generate new secret and force all users to re-login
   - Databases: Change passwords and restart containers

2. **Check for Unauthorized Access**
   - PayPal: Check transaction history
   - Email: Check sent emails log
   - Databases: Review audit logs
   - Application: Check for suspicious user activity

3. **Remove from Git History** (if committed)

   **Option A: BFG Repo Cleaner** (Recommended)
   ```bash
   # Install BFG
   brew install bfg  # macOS
   
   # Create passwords.txt with secrets to remove
   cat > passwords.txt << EOF
   AbwOUURIy30Sv997qpH0-Xzwb-rKENp64r2-F8jJx-GNMHcQ9ZFIwvQLjP2-sMe7-u-kg2AGAjjKtGSk
   EP--0kzPAWPT9yg5or-0qKVMWnjusLoqZ-aSIBWhtMouaWcbyORFvfXPPKKCNU77rzq4LHA_d_D9OvI7
   kmfu haci tnno kjqd
   EOF
   
   # Clean repository
   bfg --replace-text passwords.txt
   git reflog expire --expire=now --all
   git gc --prune=now --aggressive
   
   # Force push (CAUTION!)
   git push --force
   ```

   **Option B: Filter-Branch** (More control)
   ```bash
   git filter-branch --force --index-filter \
     "git rm --cached --ignore-unmatch Cinemax/docker-compose.override.yml" \
     --prune-empty --tag-name-filter cat -- --all
   
   git push --force --all
   ```

4. **Notify Team Members**
   - If repository was shared, notify all team members
   - Ensure they pull the cleaned history
   - Verify no one has old credentials cached

---

## 📋 Security Checklist

### Development Environment
- [ ] `.env` file created and configured
- [ ] `.env` file is gitignored
- [ ] Using sandbox/test credentials only
- [ ] No production credentials in development
- [ ] Google service account has minimal permissions

### Before Committing Code
- [ ] No hardcoded credentials in code
- [ ] `.env` not staged for commit
- [ ] Service account JSON files not staged
- [ ] Run: `git status` to verify

### Before Production Deployment
- [ ] All production credentials rotated
- [ ] Using cloud secret management (not .env)
- [ ] HTTPS/TLS enabled on all services
- [ ] Database passwords are 20+ characters
- [ ] JWT secret is cryptographically secure
- [ ] Different credentials than development
- [ ] API rate limiting configured
- [ ] Firewall rules configured
- [ ] Security audit completed

### Monthly Maintenance
- [ ] Review access logs for anomalies
- [ ] Check for dependency vulnerabilities: `dotnet list package --vulnerable`
- [ ] Update dependencies with security patches
- [ ] Verify backup systems are working
- [ ] Test disaster recovery procedures

---

## 🔍 Security Auditing

### Check for Exposed Secrets in Git

```bash
# Search for potential secrets in git history
git log --all --full-history -p -S "password" | less
git log --all --full-history -p -S "ClientSecret" | less

# Check current files for secrets
grep -r "password\|secret\|key" --include="*.yml" --include="*.json" Cinemax/
```

### Scan for Vulnerabilities

```bash
# .NET dependencies
cd Cinemax/Services/Payment/Payment.API
dotnet list package --vulnerable

# npm dependencies
cd CinemaxSPA
npm audit

# Docker images
docker scan payment.api
```

---

## 🎓 Security Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [OWASP API Security Top 10](https://owasp.org/www-project-api-security/)
- [Microsoft Security Best Practices](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [PayPal Security Best Practices](https://developer.paypal.com/api/rest/security/)
- [Docker Secrets Documentation](https://docs.docker.com/engine/swarm/secrets/)

---

## 📞 Security Contact

If you discover a security vulnerability:
1. **DO NOT** create a public GitHub issue
2. Contact the team privately
3. Allow time for a fix before public disclosure

---

**Last Updated:** October 2025
