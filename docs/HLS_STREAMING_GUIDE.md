# 🎬 HLS Streaming with Google Drive - Complete Guide

## Overview
The system now supports **HLS (HTTP Live Streaming)** with chunked video files stored in Google Drive. This enables:
- ✅ **Fast seeking** - Jump to any part of the video instantly
- ✅ **Adaptive streaming** - Load only needed chunks
- ✅ **Better performance** - No need to download entire file
- ✅ **Bandwidth efficient** - Stream chunks on demand

## Google Drive Folder Structure

### **Main Folder:**
- ID: `1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4`
- URL: https://drive.google.com/drive/u/0/folders/1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4

### **Movie Folders (Inside Main Folder):**

Each movie should be a **folder** containing HLS streaming files:

```
Main Folder/
├── The Matrix/                    <- Folder name = Movie title
│   ├── playlist.m3u8             <- HLS playlist (required)
│   ├── output.m3u8               <- Alternative name (also detected)
│   ├── segment0.ts               <- Video chunk 0
│   ├── segment1.ts               <- Video chunk 1
│   ├── segment2.ts               <- Video chunk 2
│   └── ...                       <- More chunks
│
├── Ocean's Eleven/
│   ├── playlist.m3u8
│   ├── chunk000.ts
│   ├── chunk001.ts
│   └── ...
│
└── Avatar/
    ├── output.m3u8
    ├── stream0.ts
    ├── stream1.ts
    └── ...
```

## How to Create HLS Chunks

### **Option 1: Using FFmpeg** (Recommended)

```bash
# Convert MP4 to HLS with 10-second segments
ffmpeg -i "The Matrix.mp4" \
  -codec: copy \
  -start_number 0 \
  -hls_time 10 \
  -hls_list_size 0 \
  -f hls \
  "playlist.m3u8"

# This creates:
# - playlist.m3u8 (manifest)
# - playlist0.ts, playlist1.ts, playlist2.ts, ... (chunks)
```

### **Option 2: Custom Chunk Duration**

```bash
# 5-second chunks (more granular seeking)
ffmpeg -i input.mp4 \
  -hls_time 5 \
  -hls_playlist_type vod \
  -hls_segment_filename "segment%03d.ts" \
  playlist.m3u8

# 15-second chunks (fewer files)
ffmpeg -i input.mp4 \
  -hls_time 15 \
  -hls_playlist_type vod \
  -hls_segment_filename "chunk%d.ts" \
  output.m3u8
```

### **Option 3: With Quality Options**

```bash
# Convert with specific codec settings
ffmpeg -i input.mp4 \
  -c:v libx264 -c:a aac \
  -hls_time 10 \
  -hls_playlist_type vod \
  -hls_segment_type mpegts \
  -hls_segment_filename "stream_%03d.ts" \
  playlist.m3u8
```

## Uploading to Google Drive

### **Step 1: Create Movie Folder**
1. Go to your main folder: https://drive.google.com/drive/u/0/folders/1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4
2. Click "New" → "Folder"
3. Name it **exactly** as the movie title in your catalog
   - Example: "The Matrix" (must match payment records)

### **Step 2: Upload HLS Files**
1. Open the movie folder you just created
2. Upload ALL generated files:
   - The `.m3u8` playlist file
   - ALL `.ts` chunk files
3. Wait for upload to complete

### **Step 3: Verify**
Check that the folder contains:
- ✅ At least one `.m3u8` file (playlist.m3u8 or output.m3u8)
- ✅ Multiple `.ts` files (the video chunks)

## How It Works

### **1. Movie Discovery**
- System scans main Google Drive folder for **folders** (not files)
- Each folder name = Movie title
- Only folders with `.m3u8` playlist files are considered valid movies

### **2. HLS Streaming Flow**

```
User clicks play
    ↓
Frontend requests: /Movie/stream/{folderId}
    ↓
Backend redirects to: /Movie/hls/{folderId}/playlist.m3u8
    ↓
Backend finds playlist.m3u8 in folder
    ↓
Backend modifies playlist URLs to point to /Movie/hls/{folderId}/{chunk.ts}
    ↓
Frontend (Shaka Player) parses playlist
    ↓
Shaka requests chunks: /Movie/hls/{folderId}/segment0.ts
    ↓
Backend finds chunk file in folder
    ↓
Backend streams chunk from Google Drive
    ↓
Video plays with seekable chunks!
```

### **3. Authorization**
Every request (playlist + chunks) includes:
- ✅ JWT token authentication
- ✅ Purchase verification (user must have bought the movie)
- ✅ Access control per movie folder

## API Endpoints

### **List Movies** (Shows Folders)
```
GET /Movie/movies
Authorization: Bearer {token}

Response: [
  {
    "id": "folder_id_123",  // Google Drive folder ID
    "title": "The Matrix",  // Folder name
    ...
  }
]
```

### **Stream Movie** (Redirects to HLS)
```
GET /Movie/stream/{folderId}
Authorization: Bearer {token}

Redirects to: /Movie/hls/{folderId}/playlist.m3u8
```

### **Get HLS Playlist**
```
GET /Movie/hls/{folderId}/playlist.m3u8
Authorization: Bearer {token}

Returns modified .m3u8 file with URLs pointing to our API
```

### **Get Video Chunk**
```
GET /Movie/hls/{folderId}/segment0.ts
Authorization: Bearer {token}

Returns: video chunk as video/mp2t
```

## Frontend Changes

### **Video Player**
- Restored **Shaka Player** for HLS support
- Automatically adds JWT token to all requests (playlist + chunks)
- Supports seeking, fast-forwarding, and adaptive playback
- Loads chunks on-demand

### **Benefits**
- ✅ Instant seeking to any point in video
- ✅ Only loads visible chunks (saves bandwidth)
- ✅ Better user experience
- ✅ Works like Netflix/YouTube

## Matching Movies to Purchases

The system matches based on **folder name = movie title**:

| Payment Record | Google Drive Folder | Match |
|---------------|-------------------|-------|
| "The Matrix" | `The Matrix/` | ✅ Match |
| "The Matrix" | `the matrix/` | ✅ Match (case-insensitive) |
| "Ocean's Eleven" | `Ocean's Eleven/` | ✅ Match |
| "The Matrix" | `Matrix/` | ❌ No match |

## Complete Example

### **1. User Purchases "The Matrix"**
- User buys "The Matrix" from catalog
- Payment record created with `MovieName: "The Matrix"`

### **2. Admin Prepares Video**
```bash
# Convert to HLS
ffmpeg -i "The.Matrix.1999.mp4" \
  -hls_time 10 \
  -hls_playlist_type vod \
  -hls_segment_filename "segment%03d.ts" \
  playlist.m3u8
```

### **3. Admin Uploads to Google Drive**
1. Create folder named: `The Matrix`
2. Upload:
   - `playlist.m3u8`
   - `segment000.ts`
   - `segment001.ts`
   - ... (all chunks)

### **4. User Accesses**
1. Login as premium user
2. Go to `/sessions`
3. See "The Matrix" in the list
4. Click to play
5. Video streams with HLS chunks! ✅

## Troubleshooting

### **"No movies found"**
- **Check**: Main folder contains movie folders (not individual files)
- **Check**: Each folder has a `.m3u8` file
- **Solution**: Create folders and add playlist files

### **"Movie not found or you don't have access"**
- **Check**: Folder name matches purchased movie title exactly
- **Check**: User has completed payment for this movie
- **Solution**: Verify payment records and folder names match

### **Video won't play / Shaka Player error**
- **Check**: `.m3u8` file is valid HLS playlist
- **Check**: All `.ts` chunks referenced in playlist exist in folder
- **Check**: Browser console for specific error codes
- **Solution**: Re-generate HLS files with FFmpeg

### **Chunks not loading**
- **Check**: All `.ts` files are uploaded to Google Drive
- **Check**: File names in `.m3u8` match actual file names
- **Check**: Network tab shows 200 OK for chunks
- **Solution**: Ensure all files uploaded, check logs

### **How to verify HLS structure:**
```bash
# Check what files are in a folder
# (Login and get folder ID from /sessions page)

TOKEN=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test@1234"}' | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

# Try to get playlist
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:8005/Movie/hls/FOLDER_ID/playlist.m3u8
```

## Performance Tips

### **Chunk Duration**
- **5 seconds**: Very responsive seeking, more files to manage
- **10 seconds**: Good balance (recommended)
- **15 seconds**: Fewer files, slightly slower seeking

### **File Size**
- Each 10-second chunk ≈ 2-5 MB (depending on quality)
- 2-hour movie ≈ 720 chunks ≈ 1.5-3.5 GB total

### **Bandwidth**
- User only downloads chunks they watch
- Seeking loads new chunks immediately
- Pausing stops chunk downloads

## Migration from Single Files

If you currently have single MP4 files in Google Drive:

1. **Download the MP4** locally
2. **Convert to HLS** using FFmpeg
3. **Create folder** with movie name
4. **Upload HLS files** to folder
5. **Delete old MP4** file
6. **Test** streaming

## Benefits Over Single File Approach

| Feature | Single MP4 | HLS Chunks |
|---------|-----------|------------|
| Seeking | ❌ Slow | ✅ Instant |
| Bandwidth | ❌ Full download | ✅ On-demand |
| Fast-forward | ❌ Must buffer | ✅ Skip chunks |
| User Experience | ⚠️ Basic | ✅ Professional |
| Server Load | ❌ High memory | ✅ Efficient |

## Next Steps

1. **Convert your existing videos** to HLS format
2. **Create folders** in Google Drive with movie titles
3. **Upload HLS files** to respective folders
4. **Test** the new streaming experience

Your users will now have a **Netflix-like streaming experience** with instant seeking! 🚀

