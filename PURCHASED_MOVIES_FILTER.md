# 🎬 Purchased Movies Filter for Private Sessions

## Overview
Private Sessions (Google Drive movies) are now filtered to show only movies that the user has purchased. This prevents unauthorized access to premium content.

## How It Works

### **1. Purchase Verification Flow:**

```
User → /sessions → PrivateSession API → Payment API → Filter Google Drive Movies
```

1. User accesses `/sessions` page
2. Frontend sends JWT token with request
3. PrivateSession API extracts username from JWT
4. Calls Payment API to get user's purchased movies
5. Fetches all movies from Google Drive
6. **Filters** to only show movies the user has purchased
7. Returns filtered list to frontend

### **2. Matching Logic:**

Movies are matched by **Title** (case-insensitive):
- **Payment Record**: `MovieName` (e.g., "Ocean's Eleven")
- **Google Drive File**: File name without extension (e.g., "Ocean's Eleven.mp4" → "Ocean's Eleven")

**Important:** Google Drive file names should match the movie titles in your catalog!

## Backend Changes

### **1. New Service: PaymentService**
- Location: `Services/PrivateSessions/PrivateSession/Services/PaymentService.cs`
- Purpose: Calls Payment API to get user's purchased movies
- Returns: List of purchased movie names

### **2. Updated MovieService**
- Added `username` parameter to methods
- Filters Google Drive movies based on purchases
- Returns empty list if user has no purchases

### **3. Updated MovieController**
- Extracts username from JWT claims (`ClaimTypes.Name`)
- Passes username to service methods
- Returns 403 Forbidden if user hasn't purchased a movie

### **4. Configuration**
```json
{
  "PaymentApi": {
    "BaseUrl": "http://payment.api:8080"
  }
}
```

### **5. Docker Dependencies**
PrivateSession API now depends on Payment API:
```yaml
depends_on:
  - payment.api
```

## API Endpoints Behavior

### **GET /Movie/movies**
- **Before**: Returns all movies from Google Drive
- **After**: Returns only movies the authenticated user has purchased

### **GET /Movie/{id}**
- **Before**: Returns any movie by ID
- **After**: Returns movie only if user has purchased it (404 otherwise)

### **GET /Movie/stream/{id}**
- **Before**: Streams any video
- **After**: Streams only if user has purchased the movie (403 Forbidden otherwise)

## Testing Guide

### **Step 1: Purchase a Movie**

1. Login as a buyer (e.g., `testuser`)
2. Go to catalog: http://localhost:4200/catalog
3. Add a movie to basket
4. Go to checkout and complete payment
5. Note the movie name (e.g., "The Matrix")

### **Step 2: Add Matching Video to Google Drive**

1. Upload a video file to Google Drive folder: `1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4`
2. **IMPORTANT**: Name the file to match the purchased movie title
   - Example: If you purchased "The Matrix", name the file: `The Matrix.mp4`
   - Or: `the-matrix.mp4` (case-insensitive matching)

### **Step 3: Access Private Sessions**

1. Make sure user is premium (required for /sessions access)
2. Go to: http://localhost:4200/sessions
3. You should see **ONLY** the movies you've purchased
4. Click to play - video should stream successfully

### **Step 4: Test Access Control**

**Scenario 1: User with purchases**
```bash
# Login
TOKEN=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test@1234"}' | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

# Should return purchased movies only
curl -H "Authorization: Bearer $TOKEN" http://localhost:8005/Movie/movies
```

**Scenario 2: User without purchases**
- Should return empty array `[]`
- No movies visible in /sessions page

**Scenario 3: Trying to access unpurchased movie**
```bash
curl -H "Authorization: Bearer $TOKEN" http://localhost:8005/Movie/stream/UNPURCHASED_FILE_ID
# Should return: 403 Forbidden
```

## File Naming Convention

For proper matching, Google Drive files should follow this pattern:

| Catalog Movie Title | Google Drive File Name | Match Result |
|-------------------|----------------------|--------------|
| "The Matrix" | `The Matrix.mp4` | ✅ Matches |
| "The Matrix" | `the-matrix.mp4` | ✅ Matches (case-insensitive) |
| "The Matrix" | `THE MATRIX.mp4` | ✅ Matches |
| "Ocean's Eleven" | `Ocean's Eleven.mp4` | ✅ Matches |
| "The Matrix" | `Matrix.mp4` | ❌ No match |

## Integration Points

### **Payment API Endpoint Used:**
```
GET /Payment/get-payments/{username}
```

Returns all payments with payment items (movies) for a user.

### **Response Format:**
```json
[
  {
    "id": 1,
    "buyerUsername": "testuser",
    "paymentItems": [
      {
        "movieName": "The Matrix",
        "movieId": "12345",
        "price": 9.99,
        "quantity": 1
      }
    ]
  }
]
```

## Benefits

✅ **Access Control**: Users can only watch movies they've purchased  
✅ **Premium Content Protection**: Prevents unauthorized streaming  
✅ **Automatic Filtering**: No manual management needed  
✅ **Scalable**: Works with any number of movies  
✅ **Case-Insensitive**: Flexible file naming  

## Troubleshooting

### "No movies found" but I purchased movies
- **Check**: Google Drive file names match purchased movie titles
- **Solution**: Rename files to match exact titles from catalog

### Movies show in /sessions but won't play
- **Check**: Payment records exist for the user
- **Check**: Movie title matching is correct
- **Solution**: Verify purchase completed successfully

### How to check user's purchases:
```bash
curl http://localhost:8004/Payment/get-payments/testuser
```

## Admin Override (Optional)

If you want admins to see all movies without purchase restrictions, you can update the controller to skip filtering for Admin role:

```csharp
var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
if (roles.Contains("Admin"))
{
    // Skip filtering for admins
    var movies = await _movieService.GetAllMoviesAsync();
}
else
{
    // Filter for regular users
    var movies = await _movieService.GetAllMoviesAsync(username);
}
```

This is not currently implemented but can be added if needed.

