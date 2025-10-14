# Testing Google Drive Movie Fetching

## Prerequisites
1. Videos uploaded to Google Drive folder: `1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d`
2. Docker containers running (you've already done this ✅)

## Step 1: Get a JWT Token from Identity API

First, you need to register a user or login with an existing one:

### Register a new user (if you don't have one):
```bash
curl -X POST http://localhost:4000/api/v1/Authentication/RegisterBuyer \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test@1234",
    "firstName": "Test",
    "lastName": "User"
  }'
```

### Login to get JWT token:
```bash
curl -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test@1234"
  }'
```

**Response will look like:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "..."
}
```

**Copy the `accessToken` value - you'll need it for the next steps!**

## Step 2: Test the Movies Endpoint

### Get all movies from Google Drive:
```bash
curl -X GET http://localhost:8005/Movie/movies \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

### Get a specific movie by ID:
```bash
curl -X GET http://localhost:8005/Movie/{GOOGLE_DRIVE_FILE_ID} \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

### Get movie thumbnail:
```bash
curl http://localhost:8005/Movie/imageForMovie/{GOOGLE_DRIVE_FILE_ID}
```

### Stream a movie:
```bash
curl -X GET http://localhost:8005/Movie/stream/{GOOGLE_DRIVE_FILE_ID} \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN_HERE"
```

## ⚠️ Important: Google Drive OAuth Authentication

The Google Drive API requires OAuth authentication. When running in Docker, there's a challenge because the OAuth flow normally opens a browser.

### Solution Options:

#### Option A: Use Service Account (Recommended for Docker)
1. Create a Google Service Account in Google Cloud Console
2. Download the service account JSON credentials
3. Share your Google Drive folder with the service account email
4. Update the code to use service account authentication instead of user OAuth

#### Option B: Authenticate Locally First
1. Run the PrivateSession API locally (outside Docker) once:
   ```bash
   cd /Users/davidz/kofultet/Cinemax/Cinemax/Services/PrivateSessions/PrivateSession
   dotnet run
   ```
2. The OAuth flow will open a browser
3. Complete the authentication
4. A `token.json` file will be created
5. Copy this file into the Docker container

#### Option C: Check Container Logs for OAuth URL
The Google Drive API might output an OAuth URL in the logs that you can manually visit:
```bash
docker logs privatesession.api -f
```

## Quick Test Script

Save this as `test-movies.sh`:

```bash
#!/bin/bash

# Step 1: Login and get token
echo "Logging in..."
TOKEN_RESPONSE=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "password": "Test@1234"
  }')

# Extract access token (requires jq - install with: brew install jq)
ACCESS_TOKEN=$(echo $TOKEN_RESPONSE | jq -r '.accessToken')

echo "Access Token: $ACCESS_TOKEN"
echo ""

# Step 2: Get movies from Google Drive
echo "Fetching movies from Google Drive..."
curl -X GET http://localhost:8005/Movie/movies \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  | jq '.'
```

Make it executable and run:
```bash
chmod +x test-movies.sh
./test-movies.sh
```

## Checking Google Drive Folder

Make sure you have video files in your Google Drive folder:
- Folder ID: `1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d`
- Link: https://drive.google.com/drive/folders/1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d

The service will look for any files with MIME type containing "video/".

## Expected Response

When successful, you should get a JSON array of movies:
```json
[
  {
    "id": "1ABC...xyz",
    "title": "Movie Title",
    "description": "Movie description or 'No description available'",
    "imageUrl": "https://drive.google.com/thumbnail?id=1ABC...xyz&sz=w400",
    "releaseDate": "2024-01-01T00:00:00",
    "expiresInDays": 30
  }
]
```

## Troubleshooting

### Empty Response or 401 Unauthorized
- Check if JWT token is valid and not expired (tokens expire after 15 minutes)
- Verify the Authorization header is correctly formatted

### Google Drive Authentication Error
- Check container logs: `docker logs privatesession.api -f`
- Look for OAuth URL or authentication errors
- Consider using service account authentication

### No Movies Returned
- Verify videos exist in Google Drive folder
- Check that files are video files (not folders or other file types)
- Look at container logs for any errors

### File Not Found Error
- Ensure the credentials file exists in the container
- Verify the Google Drive folder ID is correct

