# 🔐 Google Drive Service Account Setup

## Why Service Account?
Service accounts don't require browser OAuth and work perfectly in Docker containers. No user interaction needed!

## Step 1: Create Service Account

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Select your project or create a new one
3. Enable **Google Drive API**:
   - Go to **APIs & Services** > **Library**
   - Search for "Google Drive API"
   - Click **Enable**

4. Create Service Account:
   - Go to **APIs & Services** > **Credentials**
   - Click **Create Credentials** > **Service Account**
   - Name: `cinemax-drive-service`
   - Click **Create and Continue**
   - Skip optional steps, click **Done**

5. Create Key:
   - Click on the service account you just created
   - Go to **Keys** tab
   - Click **Add Key** > **Create New Key**
   - Choose **JSON**
   - Click **Create**
   - A JSON file will download automatically

6. Rename the downloaded file to:
   ```
   google-drive-service-account.json
   ```

## Step 2: Share Folder with Service Account

1. Open the downloaded JSON file
2. Find the `client_email` field (looks like: `cinemax-drive-service@your-project.iam.gserviceaccount.com`)
3. Copy this email address

4. Go to your Google Drive folder:
   https://drive.google.com/drive/u/0/folders/1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4

5. Click the **Share** button (top right)
6. Paste the service account email
7. Set permission to **Viewer**
8. **UNCHECK** "Notify people" (service accounts don't need notifications)
9. Click **Share** or **Send**

## Step 3: Copy Service Account File to Project

```bash
# Copy your downloaded service account file to the PrivateSession directory
cp ~/Downloads/google-drive-service-account.json /Users/davidz/kofultet/Cinemax/Cinemax/Services/PrivateSessions/PrivateSession/
```

## Step 4: Update Configuration

The config files need to point to your service account file:

```bash
# Update appsettings.json
# Change CredentialsPath from:
#   "client_secret_240719826823-pkttl7c8275o0fhcon27taqe8l7158ng.apps.googleusercontent.com.json"
# To:
#   "google-drive-service-account.json"
```

Or I can do this automatically for you!

## Step 5: Rebuild Docker Container

```bash
cd /Users/davidz/kofultet/Cinemax/Cinemax
docker-compose build privatesession.api
docker-compose up -d privatesession.api
```

## Step 6: Test

```bash
# Get JWT token
TOKEN=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test@1234"}' | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

# Test API
curl -H "Authorization: Bearer $TOKEN" http://localhost:8005/Movie/movies | jq '.'
```

You should see your MP4 file listed!

## Troubleshooting

**Error: "Credentials file not found"**
- Make sure the JSON file is in the PrivateSession directory
- Check the filename matches the config

**Error: "User does not have sufficient permissions"**
- Make sure you shared the folder with the service account email
- Check that the permission is at least "Viewer"

**Error: "Access denied"**
- The folder might be in a different Google Workspace
- Make sure the service account project has Drive API enabled

## Security Notes

⚠️ **Important:**
- Add `google-drive-service-account.json` to `.gitignore` (already done)
- Never commit service account keys to version control
- Rotate keys periodically for security
- Use different service accounts for dev/staging/production

