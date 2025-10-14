# 🔐 Admin-Only Movie Management

## Overview
Movie catalog operations (Add, Edit, Delete) are now restricted to Admin users only. Regular users (Buyers) can only view and purchase movies.

## Backend Changes

### 1. **MovieCatalog.API - JWT Authentication Added**

#### Package Added:
- `Microsoft.AspNetCore.Authentication.JwtBearer` (v9.0.0)

#### Program.cs Updates:
- Added JWT authentication configuration
- Added authorization middleware
- Configured to use same JWT settings as Identity API

#### Environment Variables (docker-compose.override.yml):
```yaml
- "JwtSettings:secretKey=MyVerySecretMessageWhichIsVeryVeryLongAndCannotBeBroken"
- "JwtSettings:validIssuer=Cinemax identity"
- "JwtSettings:validAudience=Cinemax"
```

### 2. **MovieCatalogController - Admin Authorization**

Protected endpoints with `[Authorize(Roles = "Admin")]`:

| Endpoint | Method | Description | Authorization |
|----------|--------|-------------|---------------|
| `GET /api/v1/MovieCatalog` | GET | List all movies | ✅ Public |
| `GET /api/v1/MovieCatalog/{id}` | GET | Get movie by ID | ✅ Public |
| `GET /api/v1/MovieCatalog/Genre/{genre}` | GET | Get by genre | ✅ Public |
| `POST /api/v1/MovieCatalog` | POST | Create movie | 🔒 **Admin Only** |
| `PUT /api/v1/MovieCatalog` | PUT | Update movie | 🔒 **Admin Only** |
| `DELETE /api/v1/MovieCatalog/{id}` | DELETE | Delete movie | 🔒 **Admin Only** |

#### Response Codes:
- `401 Unauthorized` - Not logged in
- `403 Forbidden` - Logged in but not an admin
- `200 OK` - Success (admin user)

## Frontend Changes

### 1. **Catalog Component - Role-Based UI**

#### Admin Check:
- Checks user's role from app state (JWT token)
- Sets `isAdmin` flag based on `appState.roles`
- Logs admin status to console for debugging

#### UI Elements Hidden from Non-Admins:
- ❌ "Add Movie" button
- ❌ "Edit" button on movie cards
- ❌ "Delete" button on movie cards
- ❌ Add movie form

#### Admin-Only Actions:
```typescript
*ngIf="isAdmin"  // Only show to admin users
```

### 2. **HTTP Requests - JWT Token Included**

All admin operations now include Authorization header:
```typescript
headers: {
  'Authorization': `Bearer ${this.authToken}`
}
```

## User Roles

### **Admin Users**
- Can view all movies ✅
- Can add new movies ✅
- Can edit existing movies ✅
- Can delete movies ✅
- Can purchase movies ✅

### **Buyer Users (Non-Admin)**
- Can view all movies ✅
- Can purchase movies ✅
- **Cannot** add movies ❌
- **Cannot** edit movies ❌
- **Cannot** delete movies ❌

## Testing

### Test as Admin:
1. **Create Admin User:**
   ```bash
   curl -X POST http://localhost:4000/api/v1/Authentication/RegisterAdministrator \
     -H "Content-Type: application/json" \
     -d '{
       "username": "admin",
       "email": "admin@cinemax.com",
       "password": "Admin@1234",
       "firstName": "Admin",
       "lastName": "User"
     }'
   ```

2. **Login as Admin** at frontend: http://localhost:4200/login
   - Username: `admin`
   - Password: `Admin@1234`

3. **Go to Catalog:** http://localhost:4200/catalog
   - You should see "Add Movie" button
   - You should see "Edit" and "Delete" buttons on each movie

### Test as Buyer:
1. **Login as Buyer** at frontend: http://localhost:4200/login
   - Username: `testuser`
   - Password: `Test@1234`

2. **Go to Catalog:** http://localhost:4200/catalog
   - You should **NOT** see "Add Movie" button
   - You should **NOT** see "Edit" and "Delete" buttons
   - You can only view and add to basket

### Test API Directly:

```bash
# Get admin token
ADMIN_TOKEN=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@1234"}' | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

# Try to add a movie (should work for admin)
curl -X POST http://localhost:8000/api/v1/MovieCatalog \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -d '{"Title":"Test Movie","Genre":"Action","Director":"Test","Length":120,"Price":9.99,"Rating":"PG-13","Actors":"Test Actor","Description":"Test description","ImageUrl":"http://example.com/image.jpg","linkToTrailer":"http://example.com"}'

# Get buyer token
BUYER_TOKEN=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test@1234"}' | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

# Try to add a movie (should fail with 403 Forbidden)
curl -X POST http://localhost:8000/api/v1/MovieCatalog \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $BUYER_TOKEN" \
  -d '{"Title":"Test Movie","Genre":"Action","Director":"Test","Length":120,"Price":9.99,"Rating":"PG-13","Actors":"Test Actor","Description":"Test description","ImageUrl":"http://example.com/image.jpg","linkToTrailer":"http://example.com"}'
```

## Security Features

✅ **Backend Protection**: All create/update/delete operations require Admin role
✅ **Frontend UI**: Admin buttons hidden from non-admin users
✅ **JWT Validation**: Tokens validated on every request
✅ **Role-Based Access**: Uses ASP.NET Core's built-in role-based authorization

## Notes

- Admin status is determined by the `Admin` role in the JWT token
- Role is added during user registration (RegisterAdministrator vs RegisterBuyer)
- Frontend checks role from app state (no additional API call needed)
- All changes work seamlessly with existing authentication system

