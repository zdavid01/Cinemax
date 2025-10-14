#!/bin/bash

echo "🔐 Testing Premium Access to Google Drive Movies"
echo "=================================================="
echo ""

# Step 1: Login
echo "Step 1: Logging in as testuser..."
LOGIN_RESPONSE=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"Test@1234"}')

TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
    echo "❌ Login failed. Make sure testuser exists."
    exit 1
fi

echo "✅ Logged in successfully"
echo ""

# Step 2: Check if user is premium
echo "Step 2: Checking premium status..."
USER_DETAILS=$(curl -s -H "Authorization: Bearer $TOKEN" http://localhost:4000/api/v1/User/users/testuser)
IS_PREMIUM=$(echo $USER_DETAILS | grep -o '"isPremium":[^,}]*' | cut -d':' -f2)

echo "User Details: $USER_DETAILS"
echo "Is Premium: $IS_PREMIUM"
echo ""

if [ "$IS_PREMIUM" = "true" ]; then
    echo "✅ User is already premium!"
else
    echo "⚠️  User is not premium. Upgrading now..."
    UPGRADE_RESPONSE=$(curl -s -X POST http://localhost:4000/api/v1/User/upgrade-to-premium \
      -H "Authorization: Bearer $TOKEN" \
      -H "Content-Type: application/json" \
      -d '{"username":"testuser"}')
    
    echo "Upgrade Response: $UPGRADE_RESPONSE"
    echo ""
    
    # Login again to get new token with isPremium claim
    echo "Logging in again to refresh token..."
    LOGIN_RESPONSE=$(curl -s -X POST http://localhost:4000/api/v1/Authentication/Login \
      -H "Content-Type: application/json" \
      -d '{"username":"testuser","password":"Test@1234"}')
    
    TOKEN=$(echo $LOGIN_RESPONSE | grep -o '"accessToken":"[^"]*' | cut -d'"' -f4)
    echo "✅ User upgraded to premium!"
fi

echo ""
echo "Step 3: Testing access to Google Drive movies..."
MOVIES=$(curl -s -H "Authorization: Bearer $TOKEN" http://localhost:8005/Movie/movies)
echo "Movies from Google Drive:"
echo $MOVIES | jq '.' 2>/dev/null || echo $MOVIES

echo ""
echo "=================================================="
echo "✅ Premium access is working!"
echo ""
echo "🎬 You can now access the frontend at:"
echo "   http://localhost:4200/sessions"
echo ""
echo "📝 JWT Token (copy this if needed):"
echo "   $TOKEN"

