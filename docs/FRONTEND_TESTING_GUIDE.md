# 🎬 Frontend Testing Guide - Google Drive Video Streaming

## Overview
The frontend has been updated to stream videos directly from Google Drive through the PrivateSession API.

## What's Been Updated

### 1. **Movie Streaming Service** (`movie-streaming.service.ts`)
- Updated API URL to use port **8005** (Docker)
- Added methods for getting images and stream URLs
- Now supports Google Drive movie metadata (releaseDate, expiresInDays)

### 2. **Private Sessions List** (`/sessions`)
- Displays all videos from Google Drive folder
- Shows loading state while fetching
- Displays error messages if authentication fails
- Beautiful grid layout with movie cards
- Shows Google Drive badge on each movie

### 3. **Video Player** (`/session/:id`)
- Streams videos directly from Google Drive
- Shows movie title and description
- Includes chat functionality
- Handles authentication with JWT token
- Displays loading and error states

### 4. **Routes Available**
- `/sessions` - List all Google Drive movies
- `/session/:fileId` - Play a specific movie by Google Drive file ID

## 📋 Testing Steps

### Step 1: Make Sure Backend is Running

```bash
cd /Users/davidz/kofultet/Cinemax/Cinemax
docker-compose ps
```

All containers should be running, especially `privatesession.api` on port **8005**.

### Step 2: Authenticate with Google Drive

**Important:** You must authenticate with Google Drive first!

```bash
cd /Users/davidz/kofultet/Cinemax
./authenticate-google-drive.sh
```

Follow the browser OAuth flow, then:

```bash
# Copy the generated token to Docker
docker cp /Users/davidz/kofultet/Cinemax/Cinemax/Services/PrivateSessions/PrivateSession/token.json privatesession.api:/app/

# Restart the container
cd /Users/davidz/kofultet/Cinemax/Cinemax
docker-compose restart privatesession.api
```

### Step 3: Start the Angular Frontend

```bash
cd /Users/davidz/kofultet/Cinemax/CinemaxSPA
npm start
```

The app will open at `http://localhost:4200`

### Step 4: Login to Get JWT Token

1. Navigate to `http://localhost:4200/login`
2. Login with credentials:
   - Username: `testuser`
   - Password: `Test@1234`
   
   Or register a new user at `/register`

### Step 5: Access Google Drive Movies

1. **View Movie List:**
   - Navigate to `http://localhost:4200/sessions`
   - You should see all videos from your Google Drive folder
   - Each movie shows:
     - Thumbnail from Google Drive
     - Title (filename without extension)
     - Description
     - Release date

2. **Play a Movie:**
   - Click on any movie title
   - The video player will load
   - Video streams directly from Google Drive
   - Chat panel on the right side

### Step 6: Verify Everything Works

#### ✅ Expected Behavior:
- Movies list loads without errors
- Thumbnails display correctly
- Clicking a movie navigates to player
- Video plays smoothly
- JWT authentication works
- Chat loads on the side

#### ❌ Common Issues:

**"Failed to load movies from Google Drive"**
- Solution: Make sure you've authenticated (Step 2)
- Check that `privatesession.api` container is running
- Verify Google Drive folder has video files

**"401 Unauthorized"**
- Solution: Login again to get a fresh JWT token
- Tokens expire after 15 minutes

**Video won't play**
- Solution: Check browser console for errors
- Verify the backend can redirect to Google Drive
- Make sure Google Drive file is accessible

**CORS errors**
- Solution: The backend should handle Google Drive redirects
- Check that Authorization header is being sent

## 🔧 Configuration

### API Endpoints Being Used:

| Endpoint | Purpose | Port |
|----------|---------|------|
| `GET /Movie/movies` | List all videos | 8005 |
| `GET /Movie/{id}` | Get movie metadata | 8005 |
| `GET /Movie/imageForMovie/{id}` | Get thumbnail | 8005 |
| `GET /Movie/stream/{id}` | Stream video | 8005 |

### Google Drive Folder:
- **Folder ID:** `1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4`
- **URL:** https://drive.google.com/drive/u/0/folders/1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4

## 📝 Adding Navigation Link

To make it easier to access, you can add a navigation link in your app. Update your navigation component to include:

```html
<a routerLink="/sessions" class="nav-link">
  🎬 Google Drive Movies
</a>
```

## 🎥 Supported Video Formats

The Google Drive API will return files with MIME types containing "video/":
- MP4 (video/mp4)
- WebM (video/webm)
- MKV (video/x-matroska)
- AVI (video/x-msvideo)
- MOV (video/quicktime)
- And more...

## 🐛 Debugging Tips

### Check Backend Logs:
```bash
docker logs privatesession.api -f
```

### Check Frontend Console:
Open browser DevTools (F12) and look for:
- Network requests to `http://localhost:8005`
- Console errors
- Movie data being logged

### Test API Directly:
```bash
# Get JWT token
TOKEN=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test@1234"}' | jq -r '.accessToken')

# List movies
curl -H "Authorization: Bearer $TOKEN" http://localhost:8005/Movie/movies | jq '.'

# Get specific movie
curl -H "Authorization: Bearer $TOKEN" http://localhost:8005/Movie/{FILE_ID} | jq '.'
```

## 🎉 Success!

If everything works:
1. ✅ Movies load from Google Drive
2. ✅ Thumbnails display
3. ✅ Videos play smoothly
4. ✅ Chat functionality works
5. ✅ Authentication is seamless

Enjoy streaming from Google Drive! 🍿

