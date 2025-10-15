# 🔑 Development Credentials - Quick Reference

## Overview

This project includes **working development credentials** in the repository to make onboarding instant and painless. This document explains what's available and why it's safe.

---

## ✅ What's Included & Why It's Safe

### 1. PayPal Sandbox Credentials

**Location:** `Cinemax/.env.example`

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

**Why it's safe:**
- ✅ These are **SANDBOX** credentials (test mode)
- ✅ No real money can be processed
- ✅ Transactions are simulated
- ✅ Can be rotated anytime without impact
- ✅ Designed for development sharing

**Testing payments:**
- Use the provided test account to log in to PayPal during checkout
- All transactions are fake/test data
- Account has unlimited test funds
- Perfect for integration testing
- See `docs/TESTING_PAYMENTS.md` for detailed testing guide

---

### 2. Shared Development Email

**Location:** `Cinemax/.env.example`

```env
EMAIL_ADDRESS=vida33085@gmail.com
EMAIL_APP_PASSWORD=kmfu haci tnno kjqd
```

**Why it's safe:**
- ✅ Dedicated development/testing account
- ✅ Not connected to personal/business email
- ✅ Only used for testing email functionality
- ✅ Can be changed if compromised
- ✅ All team members can monitor sent emails

**Note:** This is a Gmail **app password**, not the account password. The actual Gmail account password is secure.

---

### 3. Google Drive Service Account

**Location:** `Cinemax/Services/PrivateSessions/PrivateSession/cinemax-475115-7345803004e9.json`

**Why it's safe:**
- ✅ Service account (not a user account)
- ✅ Access limited to **one specific folder**: `1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4`
- ✅ Folder contains only test videos for streaming
- ✅ Read-only access (can't delete or modify)
- ✅ Can be revoked from Google Cloud Console instantly
- ✅ Not connected to any personal Google account

**Permissions:**
```
Google Drive Folder (Development)
└── Test Movies/
    ├── sample-video-1.mp4
    ├── sample-video-2.mp4
    └── ... (test content only)
```

---

### 4. JWT Secret (Development)

**Location:** `Cinemax/.env.example`

```env
JWT_SECRET_KEY=MyVerySecretMessageWhichIsVeryVeryLongAndCannotBeBroken
```

**Why it's safe:**
- ✅ Only for local development/testing
- ✅ All team members use the same secret (needed for token compatibility)
- ✅ Tokens are not valuable in development
- ✅ No production data in dev environment
- ⚠️ **MUST be changed for production!**

---

### 5. Database Passwords

**Location:** `Cinemax/.env.example`

```env
DB_SA_PASSWORD=MATF12345
POSTGRES_PASSWORD=MATF12345
```

**Why it's safe:**
- ✅ Databases run in local Docker containers only
- ✅ Not exposed to internet
- ✅ Only accessible on localhost
- ✅ Containers are ephemeral (can be destroyed/recreated)
- ✅ No production data

---

## 🚀 Quick Start for New Developers

```bash
# 1. Clone
git clone <repo-url>
cd Cinemax

# 2. Copy credentials (already working!)
cd Cinemax
cp .env.example .env

# 3. Start everything
docker-compose up -d

# 4. Test
curl http://localhost:4000/health  # All services have health endpoints

# Done! Everything works! 🎉
```

**No account creation needed!**  
**No API key hunting!**  
**No credential configuration!**

---

## 🛡️ Security Boundaries

### Development Environment

| Component | Credential Source | Shared? | Safe to Commit? |
|-----------|------------------|---------|-----------------|
| PayPal Sandbox | `.env.example` | ✅ Yes | ✅ Yes |
| Team Email | `.env.example` | ✅ Yes | ✅ Yes |
| Google Service Account | `cinemax-475115-*.json` | ✅ Yes | ✅ Yes |
| JWT Dev Secret | `.env.example` | ✅ Yes | ✅ Yes |
| DB Passwords | `.env.example` | ✅ Yes | ✅ Yes |

### Production Environment

| Component | Credential Source | Shared? | Safe to Commit? |
|-----------|------------------|---------|-----------------|
| PayPal Production | Secure Vault | ❌ No | ❌ NEVER |
| Production Email/SMTP | Secure Vault | ❌ No | ❌ NEVER |
| Google Service Account | Secure Vault | ❌ No | ❌ NEVER |
| JWT Secret | Secure Vault | ❌ No | ❌ NEVER |
| DB Passwords | Secure Vault | ❌ No | ❌ NEVER |

---

## 🔄 When to Rotate Credentials

### Development (Low Priority)
- When team members leave the project
- If credentials are accidentally exposed publicly
- Annually as good practice

**How to rotate:**
1. Generate new credentials
2. Update `.env.example`
3. Notify team to re-copy the file
4. Revoke old credentials

### Production (High Priority)
- Immediately if compromised
- Every 90 days (recommended)
- When employees with access leave
- After security audits

---

## ❓ FAQ

**Q: Why are credentials in the repo?**  
A: These are development-only credentials designed for team sharing. They enable instant onboarding without credential hunting.

**Q: Is this secure?**  
A: Yes, for development! All credentials are scoped to test environments, sandbox modes, or have minimal permissions. Production must use separate secure credentials.

**Q: What if someone steals these credentials?**  
A: Development credentials have:
- PayPal: Sandbox mode (no real money)
- Email: Monitored team account (no sensitive data)
- Google Drive: Limited to test folder
- Databases: Local only (not exposed)

Impact is minimal and credentials can be rotated easily.

**Q: How do I use my own credentials?**  
A: Simply edit the `.env` file after copying it. Your changes won't be committed (`.env` is gitignored).

**Q: What about production?**  
A: See `docs/SECURITY_GUIDE.md` for production deployment with proper secret management (Docker Secrets, Azure Key Vault, etc.).

---

## 📞 Questions?

If you have security concerns or find issues with the development credentials, contact the team leads.

**Team Leads:**
- Bojan Velickovic
- David Zivkovic
- Dusan Trtica
- Stefan Jevtic

---

**Last Updated:** October 2025

