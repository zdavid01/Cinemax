# Google Drive Integration for Private Sessions

## Overview
The PrivateSessions service has been updated to stream videos directly from Google Drive instead of using local files. This allows for centralized video storage and easier content management.

## What Changed

### 1. **Dependencies Added**
- `Google.Apis.Drive.v3` (v1.68.0.3533) - Google Drive API client
- `Google.Apis.Auth` (v1.68.0) - Google authentication

### 2. **Configuration**
The `appsettings.json` now includes Google Drive configuration:
```json
{
  "GoogleDrive": {
    "CredentialsPath": "client_secret_240719826823-pkttl7c8275o0fhcon27taqe8l7158ng.apps.googleusercontent.com.json",
    "FolderId": "1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d",
    "ApplicationName": "Cinemax Private Sessions"
  }
}
```

### 3. **Google Drive Service**
The `GoogleDriveService` now:
- Authenticates using OAuth2 with your Google credentials
- Lists all video files from the specified Google Drive folder
- Generates proper streaming URLs for videos
- Provides thumbnail URLs for video previews

### 4. **Movie Service**
The `MovieService` now fetches movies dynamically from Google Drive instead of using hardcoded data.

## Setup Instructions

### 1. **Install Dependencies**
Run the following command in the PrivateSession directory:
```bash
dotnet restore
```

### 2. **First-Time Authentication**
When you first run the service, it will:
1. Open a browser window for Google OAuth authentication
2. Ask you to sign in with your Google account
3. Request permission to access Google Drive (read-only)
4. Store the authentication token in `token.json` for future use

**Important**: The user you authenticate with must have access to the Google Drive folder specified in the configuration.

### 3. **Google Drive Folder Structure**
Your Google Drive folder should contain video files. The service will:
- List all video files in the folder (ID: `1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d`)
- Use the filename (without extension) as the movie title
- Use the file's creation date as the release date
- Generate thumbnails from Google Drive

### 4. **Supported Video Formats**
The service will detect any file with a MIME type containing "video/", including:
- MP4 (video/mp4)
- WebM (video/webm)
- MKV (video/x-matroska)
- AVI (video/x-msvideo)
- MOV (video/quicktime)

## API Endpoints

### Get All Movies
```
GET /Movie/movies
Authorization: Bearer {token}
```
Returns a list of all video files from Google Drive.

### Get Movie by ID
```
GET /Movie/{fileId}
Authorization: Bearer {token}
```
Returns details for a specific video file.

### Get Movie Thumbnail
```
GET /Movie/imageForMovie/{fileId}
```
Redirects to the Google Drive thumbnail URL.

### Stream Movie
```
GET /Movie/stream/{fileId}
Authorization: Bearer {token}
```
Redirects to the Google Drive streaming URL.

## How It Works

1. **Authentication**: The service uses OAuth2 to authenticate with Google Drive API
2. **File Discovery**: It queries the specified folder for video files
3. **Metadata Extraction**: For each video, it extracts:
   - File ID (used as movie ID)
   - Filename (used as title)
   - Description (if set in Google Drive)
   - Thumbnail link
   - Creation date
4. **Streaming**: Videos are streamed directly from Google Drive using their file IDs

## Troubleshooting

### "Credentials file not found" Error
- Ensure `client_secret_240719826823-pkttl7c8275o0fhcon27taqe8l7158ng.apps.googleusercontent.com.json` exists in the PrivateSession directory
- Check the `CredentialsPath` in `appsettings.json`

### "Access Denied" Error
- Make sure the authenticated Google account has access to the folder
- Verify the folder ID in `appsettings.json` is correct
- Delete `token.json` and re-authenticate

### Videos Not Showing
- Check that the Google Drive folder contains video files
- Verify the folder ID: `1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d`
- Check the application logs for errors

### Video Won't Stream
- Ensure the file is a supported video format
- Check that sharing settings allow the authenticated user to download
- Large files may take time to start streaming

## Security Notes

1. **OAuth Tokens**: The `token.json` file contains sensitive authentication data - add it to `.gitignore`
2. **Credentials**: Never commit the `client_secret_*.json` file to version control
3. **Read-Only Access**: The service only requests read permissions (`DriveService.Scope.DriveReadonly`)
4. **Authorization**: All endpoints (except thumbnails) require JWT authentication

## Future Enhancements

Consider adding:
- Folder hierarchy support for categories/genres
- Metadata from Google Drive file properties
- Caching to reduce API calls
- Batch operations for better performance
- Support for shared drives
- Video transcoding for better streaming compatibility


