# Private Sessions Service

## Google Drive Service Account

### Development Credentials

The file `cinemax-475115-7345803004e9.json` is a **shared development service account** for the team.

**To get this file:** See `GET_CREDENTIALS.md` in this directory.

**This credential is:**
- ✅ Safe to share with your development team
- ✅ Restricted to a specific development Google Drive folder
- ✅ Has read-only access to test videos
- ✅ Not connected to any personal Google account
- ✅ Can be rotated/revoked at any time

**This credential is NOT:**
- ❌ For production use
- ❌ Connected to personal data
- ❌ A security risk if shared with team members

### For Production

Create your own Google Cloud service account:
1. Go to Google Cloud Console
2. Create a new service account
3. Download the JSON key
4. Store it securely (Azure Key Vault, AWS Secrets Manager, etc.)
5. **DO NOT** commit production credentials to git

See: `GOOGLE_DRIVE_SETUP.md` for detailed instructions.

### OAuth Client Secret

The `client_secret_*.json` file is **gitignored** and should remain private. Each developer should:
1. Create their own OAuth credentials (if needed for development)
2. Download their own `client_secret_*.json`
3. Place it in this directory (it will be automatically gitignored)

---

**Note:** Service accounts are preferred over OAuth for server-to-server communication.

