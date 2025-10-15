# 🎬 Cinemax - Movie Streaming Platform

A modern, microservices-based movie streaming platform built with **.NET 9**, **Angular**, and **Docker**. Cinemax provides movie catalog browsing, user authentication, payment processing, private streaming sessions, and more.

---

## 🏗️ Architecture Overview

This project follows **Clean Architecture** and **Microservices** patterns with the following services:

### Backend Services (C# .NET 9)

| Service | Port | Description | Technology |
|---------|------|-------------|------------|
| **Identity.API** | 4000 | User authentication & authorization | ASP.NET Identity, JWT, MS SQL Server |
| **MovieCatalog.API** | 8000 | Movie catalog management | MongoDB |
| **Basket.API** | 8001 | Shopping cart management | Redis, gRPC Client |
| **Payment.API** | 8004 | Payment processing & PayPal integration | PostgreSQL, gRPC Server, RabbitMQ |
| **PrivateSession.API** | 8005 | Private movie streaming sessions | Google Drive API, SignalR |
| **Email.API** | 8006 | Email notification service | RabbitMQ Consumer, SMTP |

### Frontend (Angular)
- **CinemaxSPA** - Modern Angular single-page application with responsive design

### Infrastructure Services

| Service | Port | Description |
|---------|------|-------------|
| **RabbitMQ** | 5672, 15672 | Message broker for async communication |
| **Redis** | 6379 | Distributed cache for baskets |
| **PostgreSQL** | 5432 | Payment database |
| **MongoDB** | 27017 | Movie catalog database |
| **MS SQL Server** | 1433 | Identity database |
| **pgAdmin** | 5050 | PostgreSQL management UI |

---

## 🚀 Quick Start

### Prerequisites

1. **Docker & Docker Compose** (required)
   - Docker Desktop 4.0+ recommended
   - [Download Docker](https://www.docker.com/products/docker-desktop)

2. **For Frontend Development** (optional)
   - Node.js 18+ and npm
   - Angular CLI: `npm install -g @angular/cli`

3. **For Backend Development** (optional)
   - .NET 9 SDK
   - IDE: Visual Studio 2022, Rider, or VS Code

---

## 🔐 Security Setup

### Quick Setup for Development

**The `.env.example` file contains working development credentials ready to use!**

Simply copy the template and you're ready to go:
```bash
cd Cinemax
cp .env.example .env
docker-compose up -d
```

**What's included in `.env.example`:**
- ✅ PayPal **sandbox** credentials (test mode - no real money) 
- ✅ Shared team email for development testing
- ✅ Development-safe JWT secret
- ✅ Test database passwords
- ✅ Google Drive development folder

**Google Drive Service Account (for Private Sessions):**
- ✅ Full credentials documented in: `docs/GOOGLE_SERVICE_ACCOUNT.md`
- ✅ One-command setup available in the doc
- ✅ Restricted to development folder only
- ✅ Safe to use for development
- ℹ️ See: `Cinemax/Services/PrivateSessions/GET_CREDENTIALS.md` for setup instructions

**PayPal Sandbox Test Account (for testing payments):**
```
Email: sb-2qjuu34887226@personal.example.com
Password: 4)grxJ35
```
Use these credentials to log in to PayPal during checkout testing. This is a PayPal sandbox account - no real money will be charged.

**These credentials are:**
- ✅ Safe to share within your development team
- ✅ Safe to commit for development use
- ✅ Ready to use immediately - no setup required
- ⚠️ **NOT for production use** - see production setup guide

### For Production Deployment

⚠️ **IMPORTANT:** Before deploying to production:

1. **Generate new secure credentials:**
   ```bash
   # JWT Secret (use this in production!)
   openssl rand -base64 64
   ```

2. **Get your own API credentials:**
   - **PayPal**: Create production app at https://developer.paypal.com
   - **Gmail**: Generate app password at https://myaccount.google.com/apppasswords
   - **Databases**: Use strong random passwords (20+ chars)

3. **Update `.env`** with production values

4. **Never commit `.env` to git** (already in `.gitignore` ✅)

See `docs/SECURITY_GUIDE.md` for detailed production security practices.

---

## 🐳 Running with Docker (Recommended)

### Step 1: Clone the Repository
```bash
git clone <repository-url>
cd Cinemax
```

### Step 2: Setup Environment Variables
```bash
cd Cinemax
cp .env.example .env
```

**Note:** The `.env.example` contains working development credentials - no editing needed for testing!

### Step 3: Start All Services
```bash
cd Cinemax
docker-compose up -d
```

This will start all microservices and databases. First run may take 5-10 minutes to download images and build.

### Step 3: Verify Services are Running
```bash
docker-compose ps
```

All services should show status "Up".

### Step 4: Access the Application

**Frontend:**
- Angular SPA: `http://localhost:4200` (if running separately)

**Backend APIs:**
- Identity API: `http://localhost:4000/swagger`
- Movie Catalog API: `http://localhost:8000/swagger`
- Basket API: `http://localhost:8001/swagger`
- Payment API: `http://localhost:8004/swagger`
- Private Session API: `http://localhost:8005/swagger`
- Email API: `http://localhost:8006/swagger`

**Infrastructure:**
- RabbitMQ Management: `http://localhost:15672` (guest/guest)
- pgAdmin: `http://localhost:5050` (dev@gmail.com/admin1234)

### Step 5: Stop All Services
```bash
docker-compose down
```

To remove volumes (databases) as well:
```bash
docker-compose down -v
```

---

## 🔧 Development Setup

### Backend Development (without Docker)

1. **Start Required Infrastructure**
   ```bash
   cd Cinemax
   docker-compose up -d identitydb paymentdb moviecatalogdb basketdb rabbitmq pgadmin
   ```

2. **Run Individual Services**
   ```bash
   # Identity API
   cd Cinemax/Security/IdentityServer
   dotnet run

   # Payment API
   cd Cinemax/Services/Payment/Payment.API
   dotnet run

   # Other services follow the same pattern
   ```

### Frontend Development

1. **Install Dependencies**
   ```bash
   cd CinemaxSPA
   npm install
   ```

2. **Run Development Server**
   ```bash
   npm start
   # or
   ng serve
   ```

3. **Access Frontend**
   - Navigate to `http://localhost:4200`

---

## 🏛️ Architecture Details

### Communication Patterns

1. **Synchronous (gRPC)**
   - Basket.API → Payment.API
   - Used for real-time payment processing during checkout

2. **Asynchronous (RabbitMQ)**
   - Payment.API → Email.API
   - Basket.API → Email.API (via events)
   - Used for non-blocking email notifications

3. **HTTP REST**
   - Frontend → All Backend APIs
   - Used for standard CRUD operations

### Database Strategy

- **Database per Service** pattern
- Each microservice has its own database
- No direct database sharing between services

---

## 📦 Project Structure

```
Cinemax/
├── Cinemax/                        # Backend microservices
│   ├── Services/
│   │   ├── Basket.API/             # Shopping cart service
│   │   ├── Email.API/              # Email notification service
│   │   ├── MovieCatalog.API/       # Movie catalog service
│   │   ├── Payment/                # Payment service (Clean Architecture)
│   │   │   ├── Payment.API/        # API layer
│   │   │   ├── Payment.Application/# Business logic layer
│   │   │   ├── Payment.Domain/     # Domain models
│   │   │   └── Payment.Infrastructure/ # Data access & external services
│   │   └── PrivateSessions/        # Private streaming sessions
│   ├── Security/
│   │   └── IdentityServer/         # Authentication & authorization
│   ├── Common/
│   │   └── EventBus.Messages/      # Shared event definitions
│   ├── docker-compose.yml          # Docker services definition
│   └── docker-compose.override.yml # Development configuration
│
└── CinemaxSPA/                     # Angular frontend
    ├── src/app/                    # Application components
    ├── src/assets/                 # Static assets
    └── package.json                # npm dependencies
```

---

## 🔐 Default Credentials

### Infrastructure Services
- **RabbitMQ Management:** guest/guest
- **pgAdmin:** dev@gmail.com/admin1234
- **Database SA Password:** MATF12345

### Creating Admin Account for Testing

**Option 1: Using Swagger UI (Recommended)**

1. Navigate to Identity API Swagger: `http://localhost:4000/swagger`
2. Find the `POST /api/v1/Authentication/RegisterAdministrator` endpoint
3. Click "Try it out"
4. Enter the following JSON:
   ```json
   {
     "firstName": "Admin",
     "lastName": "User",
     "username": "admin",
     "password": "Admin123!",
     "email": "admin@cinemax.com"
   }
   ```
5. Click "Execute"
6. Use these credentials to login at `POST /api/v1/Authentication/Login`

**Option 2: Using cURL**

```bash
curl -X POST "http://localhost:4000/api/v1/Authentication/RegisterAdministrator" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Admin",
    "lastName": "User",
    "username": "admin",
    "password": "Admin123!",
    "email": "admin@cinemax.com"
  }'
```

**Option 3: Using HTTP File (Easiest)**

Use the included `test-admin.http` file in the root directory:

1. Open `test-admin.http` in VS Code (with REST Client extension) or JetBrains IDE
2. Click "Send Request" above `### Register Admin User`
3. Then click "Send Request" above `### Login as Admin`
4. Copy the returned JWT token for authenticated requests

The file includes examples for:
- ✅ Registering Admin users
- ✅ Registering Buyer users  
- ✅ Logging in
- ✅ Getting user lists (with auth)

### Creating Regular Buyer Account

Use the same methods but with the `RegisterBuyer` endpoint:
```
POST /api/v1/Authentication/RegisterBuyer
```

**Note:** Password requirements:
- Minimum 5 characters
- At least 1 digit
- Must include uppercase, lowercase, and special characters recommended

---

## 🛠️ Technology Stack

### Backend
- **.NET 9** - Web APIs
- **ASP.NET Core Identity** - Authentication
- **Entity Framework Core 9** - ORM
- **MassTransit** - RabbitMQ abstraction
- **Grpc.AspNetCore** - gRPC server/client
- **MediatR** - CQRS pattern
- **AutoMapper** - Object mapping
- **PayPal SDK** - Payment processing
- **Google Drive API** - Video streaming

### Frontend
- **Angular 18** - SPA framework
- **TypeScript** - Type-safe JavaScript
- **RxJS** - Reactive programming
- **SignalR** - Real-time communication

### Databases
- **PostgreSQL 15** - Payment data
- **MongoDB** - Movie catalog
- **MS SQL Server 2019** - Identity data
- **Redis** - Distributed cache

### Message Broker
- **RabbitMQ 3** - Asynchronous messaging

---

## 🧪 Testing the Services

### Using Swagger UI
Each service has Swagger documentation:
```
http://localhost:4000/swagger  # Identity API
http://localhost:8000/swagger  # Movie Catalog API
http://localhost:8001/swagger  # Basket API
http://localhost:8004/swagger  # Payment API
http://localhost:8005/swagger  # Private Session API
http://localhost:8006/swagger  # Email API
```

### Using HTTP Files
Each service includes `.http` files for testing:
```
Cinemax/Services/Basket.API/Basket.API.http
Cinemax/Services/Payment/Payment.API/Payment.API.http
Cinemax/Services/Email.API/Email.API.http
```

Open these in Visual Studio, Rider, or VS Code with REST Client extension.

---

## 🔄 Rebuilding Services

### Rebuild Specific Service
```bash
docker-compose build payment.api
docker-compose up -d payment.api
```

### Rebuild All Services
```bash
docker-compose build --no-cache
docker-compose up -d
```

### View Service Logs
```bash
# All services
docker-compose logs -f

# Specific service
docker-compose logs -f payment.api

# Last 50 lines
docker-compose logs --tail=50 payment.api
```

---

## 🐛 Troubleshooting

### Services Won't Start

1. **Check if ports are already in use:**
   ```bash
   # On macOS/Linux
   lsof -i :8004
   
   # On Windows
   netstat -ano | findstr :8004
   ```

2. **Clean and restart:**
   ```bash
   docker-compose down -v
   docker-compose up -d
   ```

### Database Issues

1. **Reset databases:**
   ```bash
   docker-compose down -v  # Removes volumes
   docker-compose up -d
   ```

2. **View database logs:**
   ```bash
   docker-compose logs paymentdb
   docker-compose logs identitydb
   docker-compose logs moviecatalogdb
   ```

### RabbitMQ Issues

1. **Check RabbitMQ is running:**
   ```bash
   docker-compose ps rabbitmq
   ```

2. **Access RabbitMQ Management UI:**
   - URL: `http://localhost:15672`
   - Credentials: guest/guest
   - Check queues and connections

### Build Errors

1. **Clean all build artifacts:**
   ```bash
   # From Cinemax/Cinemax directory
   find . -type d \( -name bin -o -name obj \) -exec rm -rf {} + 2>/dev/null
   ```

2. **Remove Docker build cache:**
   ```bash
   docker-compose build --no-cache
   ```

---

## 📚 Additional Documentation

### Service-Specific Documentation
- **Payment Setup:** `Cinemax/Services/Payment/Payment.API/PAYPAL_SETUP.md`
- **Email Setup:** `Cinemax/Services/Email.API/README.md`
- **Private Sessions:** 
  - Overview: `Cinemax/Services/PrivateSessions/README.md`
  - Google Drive Setup: `Cinemax/Services/PrivateSessions/PrivateSession/GOOGLE_DRIVE_SETUP.md`

### General Documentation (in `/docs`)
- **🛡️ Security Guide:** `docs/SECURITY_GUIDE.md` ⭐ **READ THIS FIRST**
- **🔑 Development Credentials:** `docs/DEVELOPMENT_CREDENTIALS.md` - Why credentials are in the repo
- **💳 Testing Payments:** `docs/TESTING_PAYMENTS.md` - PayPal sandbox testing guide
- **🔑 Google Service Account:** `docs/GOOGLE_SERVICE_ACCOUNT.md` - Google Drive credentials for dev
- **Google Drive Service Account Setup:** `docs/SERVICE_ACCOUNT_SETUP.md`
- **HLS Video Streaming Guide:** `docs/HLS_STREAMING_GUIDE.md`
- **Frontend Testing Guide:** `docs/FRONTEND_TESTING_GUIDE.md`
- **Frontend Premium Features:** `docs/FRONTEND_PREMIUM_FIX.md`
- **Admin Protection Summary:** `docs/ADMIN_PROTECTION_SUMMARY.md`
- **Purchased Movies Filter:** `docs/PURCHASED_MOVIES_FILTER.md`
- **Testing Google Drive Integration:** `docs/TESTING_GOOGLE_DRIVE.md`

---

## 🌟 Key Features

- **User Authentication** - JWT-based with role management (Admin, Buyer)
- **Movie Catalog** - Browse movies by genre with premium content
- **Shopping Cart** - Add movies to basket with Redis caching
- **Payment Processing** - PayPal integration with gRPC communication
- **Email Notifications** - Asynchronous via RabbitMQ
- **Private Streaming** - Google Drive integration with HLS streaming
- **Real-time Chat** - SignalR for private session chat
- **Admin Protection** - Secure endpoints with role-based access

---

## 🤝 Contributing

1. Create a feature branch
2. Make your changes
3. Test with Docker Compose
4. Submit a pull request

---

<!-- ## 📄 License

[Your License Here] -->

---

## 👥 Authors

Bojan Velickovic 1070/2024
David Zivkovic 1027/2024
Dusan Trtica 1041/2023
Stefan Jevtic 1043/2024

---

## 🆘 Support

For issues and questions:
- Check the troubleshooting section above
- Review service-specific README files
- Check Docker logs: `docker-compose logs [service-name]`

---

## 🛡️ Security Best Practices

### ⚠️ IMPORTANT: Before Deploying to Production

1. **Rotate ALL Credentials**
   - Generate new JWT secret with `openssl rand -base64 64`
   - Create new PayPal API credentials
   - Generate new database passwords
   - Create new email app password

2. **Use Docker Secrets or Cloud Secret Management**
   - For production: Use Docker Swarm secrets or Kubernetes secrets
   - For cloud: Use Azure Key Vault, AWS Secrets Manager, or Google Secret Manager
   - See `Cinemax/Services/Payment/Payment.API/docker-secrets-setup.md`

3. **Never Commit to Git**
   - `.env` file (already gitignored ✅)
   - Google Drive credentials (already gitignored ✅)
   - Any file containing passwords or API keys

4. **Environment-Specific Secrets**
   - Use different credentials for dev/staging/production
   - Never use production credentials in development
   - Implement secret rotation policies

5. **Additional Security Measures**
   - Enable HTTPS/TLS in production
   - Use strong database passwords (20+ characters)
   - Implement rate limiting on APIs
   - Enable firewall rules to restrict database access
   - Regular security audits

### If Secrets Are Compromised

1. **Immediately rotate the compromised credentials**
2. **Check git history** if secrets were committed:
   ```bash
   git log --all --full-history -- "*docker-compose*"
   ```
3. **Use BFG Repo Cleaner** to remove secrets from git history
4. **Force push** to remote (only if you control all copies)

---

**Last Updated:** October 2025
