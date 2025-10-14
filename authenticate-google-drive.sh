#!/bin/bash

echo "🔐 Google Drive Authentication Script"
echo "======================================"
echo ""
echo "This script will help you authenticate with Google Drive."
echo ""

cd /Users/davidz/kofultet/Cinemax/Cinemax/Services/PrivateSessions/PrivateSession

echo "Step 1: Running PrivateSession locally to complete OAuth flow..."
echo ""
echo "⚠️  A browser window will open for Google authentication."
echo "    Please sign in and grant permissions."
echo ""
echo "Press CTRL+C after you see 'Application started' and the browser opens."
echo ""

# Set the same environment variables as Docker
export ASPNETCORE_ENVIRONMENT=Development
export JwtSettings__secretKey="MyVerySecretMessageWhichIsVeryVeryLongAndCannotBeBroken"
export JwtSettings__validIssuer="Cinemax identity"
export JwtSettings__validAudience="Cinemax"
export GoogleDrive__CredentialsPath="client_secret_240719826823-pkttl7c8275o0fhcon27taqe8l7158ng.apps.googleusercontent.com.json"
export GoogleDrive__FolderId="1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4"
export GoogleDrive__ApplicationName="Cinemax Private Sessions"

# Run the application
dotnet run --urls "http://localhost:8005"

