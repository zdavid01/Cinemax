# 🔑 Google Drive Service Account - Development

## Overview

This document explains how to obtain and set up the Google Drive service account credentials for development and testing. This service account has **limited access** to a specific development folder only.

---

## 📋 Service Account Details

**Project:** cinemax-475115  
**Account Email:** cinemax@cinemax-475115.iam.gserviceaccount.com  
**Purpose:** Access development Google Drive folder for video streaming tests  
**Permissions:** Read-only access to folder `1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4`

---

## 🔐 How to Get the Credentials

**File name:** `cinemax-475115-7345803004e9.json`

**Location:** Must be placed in `Cinemax/Services/PrivateSessions/PrivateSession/`

### Option 1: Contact Team Members (Recommended)

The service account JSON file is available from any team member:
- **Bojan Velickovic** - 1070/2024
- **David Zivkovic** - 1027/2024
- **Dusan Trtica** - 1041/2023
- **Stefan Jevtic** - 1043/2024

They can send you the file via:
- Email
- Slack/Discord
- Direct message
- Shared team drive

### Option 2: Copy from Another Team Member's Setup

If a teammate has a working installation, they can share the file:

```bash
# They run this and send you the file:
cat Cinemax/Services/PrivateSessions/PrivateSession/cinemax-475115-7345803004e9.json
```

### Option 3: Copy from Existing Local Installation

If you have access to a working setup on another machine:

```bash
# From your other machine
cp /path/to/working/Cinemax/Services/PrivateSessions/PrivateSession/cinemax-475115-7345803004e9.json \
   /path/to/new/Cinemax/Services/PrivateSessions/PrivateSession/
```

### Option 4: Create Your Own (Advanced)

For advanced users who want their own service account:

See: `Cinemax/Services/PrivateSessions/PrivateSession/GOOGLE_DRIVE_SETUP.md`

---

## 📥 Setup Instructions

Once you have the JSON file:

### Step 1: Place the File

Put the file in the correct location:
```bash
# Make sure you're in the project root
cd Cinemax

# File should be here:
ls -la Services/PrivateSessions/PrivateSession/cinemax-475115-7345803004e9.json
```

### Step 2: Verify It's Gitignored

The file should **NOT** show up in `git status`:

```bash
git status  # Should not list the JSON file
```

If it does show up, verify `.gitignore` has:
```
**/cinemax-*.json
```

### Step 3: Start the Service

```bash
docker-compose up -d privatesession.api
```

### Step 4: Test It Works

```bash
# Check logs for successful Google Drive initialization
docker-compose logs privatesession.api | grep -i "google\|drive"
```

---

## ✅ Why This Service Account is Safe to Share

1. **Limited Scope**: Only accesses folder `1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4`
2. **Read-Only**: Cannot modify or delete files
3. **Test Content**: Folder contains only development test videos
4. **Revocable**: Can be disabled in Google Cloud Console instantly
5. **Service Account**: Not tied to any personal Google account
6. **Development Only**: Never use in production
7. **Gitignored**: File won't be accidentally committed

---

## 🔄 If Credentials Stop Working

The service account may be rotated or revoked. Contact team leads for updated credentials.

**Common Issues:**

1. **"Authentication failed"** - Service account may have been rotated
2. **"Access denied"** - File permissions may have changed
3. **"File not found"** - Check file is in correct location

**Solutions:**
- Get updated credentials from team
- Verify file name matches exactly
- Check file isn't corrupted during transfer

---

## 🛡️ Security Notes

- ✅ This file is **gitignored** and should never be committed
- ✅ Share via secure team channels (not public)
- ✅ These are **development credentials only**
- ❌ Never use in production
- ❌ Never share outside the development team

For production deployment, create a separate service account with production-level access and store it in a secure vault (Azure Key Vault, AWS Secrets Manager, etc.).

See `docs/SECURITY_GUIDE.md` for production security practices.

---

**Last Updated:** October 2025

