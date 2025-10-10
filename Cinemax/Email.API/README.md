# Email.API Microservice

A microservice for sending emails asynchronously using RabbitMQ message queue to prevent blocking API requests.

## Features

- **Asynchronous Email Processing**: Uses RabbitMQ to queue emails for non-blocking API responses
- **Synchronous Email Sending**: Direct email sending for testing purposes
- **Multiple Recipients**: Support for CC and BCC recipients
- **Priority Handling**: Support for different email priorities (Low, Normal, High)
- **HTML/Text Support**: Send both HTML and plain text emails
- **Attachment Support**: Send emails with file attachments
- **Health Checks**: Built-in health check endpoint
- **Logging**: Comprehensive logging with Serilog
- **Docker Support**: Containerized with Docker

## Architecture

```
Other Microservices → Email.API Controller → RabbitMQ → Email Consumer → SMTP Server
```

1. **Other microservices** send email requests to Email.API
2. **Email.API Controller** validates requests and publishes to RabbitMQ
3. **RabbitMQ** queues the email messages
4. **Email Consumer** processes messages from the queue
5. **Email Service** sends emails via SMTP

## API Endpoints

### POST /api/email/send-async
Send email asynchronously via RabbitMQ (recommended for production)

**Request Body:**
```json
{
  "to": "recipient@example.com",
  "subject": "Email Subject",
  "body": "<h1>Email Content</h1>",
  "from": "sender@example.com", // Optional, uses default if not provided
  "isHtml": true,
  "cc": ["cc1@example.com", "cc2@example.com"], // Optional
  "bcc": ["bcc@example.com"], // Optional
  "attachments": {}, // Optional, file path mappings
  "priority": 1 // 0=Low, 1=Normal, 2=High
}
```

**Response:**
```json
{
  "success": true,
  "message": "Email has been queued for sending",
  "data": "email-event-id"
}
```

### POST /api/email/send-sync
Send email synchronously (for testing purposes)

Same request body as async endpoint.

### GET /health
Health check endpoint

## Configuration

### Email Settings (appsettings.json)
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "your-email@gmail.com",
    "FromName": "Cinemax System",
    "EnableSsl": true
  }
}
```

### RabbitMQ Connection
```json
{
  "ConnectionStrings": {
    "RabbitMQ": "amqp://guest:guest@rabbitmq:5672"
  }
}
```

## Setup Instructions

### 1. Configure Email Settings
Update the email settings in `appsettings.json` or environment variables:
- For Gmail: Use App Passwords instead of regular passwords
- For other providers: Update SMTP server, port, and credentials accordingly

### 2. Run with Docker Compose
```bash
docker-compose up email.api
```

### 3. Run Locally
```bash
cd Email.API
dotnet restore
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5006
- HTTPS: https://localhost:7006
- Swagger UI: https://localhost:7006/swagger

## Usage Examples

### From Other Microservices

```csharp
// C# Example
var emailRequest = new
{
    to = "user@example.com",
    subject = "Welcome to Cinemax!",
    body = "<h1>Welcome!</h1><p>Thank you for joining Cinemax.</p>",
    isHtml = true,
    priority = 1
};

var response = await httpClient.PostAsJsonAsync(
    "http://email.api:8080/api/email/send-async", 
    emailRequest);
```

### Using HTTP Client
See `Email.API.http` file for example requests.

## Queue Configuration

The service uses the following RabbitMQ configuration:
- **Queue Name**: `send-email-queue`
- **Retry Policy**: 3 retries with 5-second intervals
- **Error Handling**: Uses in-memory outbox pattern

## Monitoring

- **Health Checks**: Available at `/health`
- **Logging**: Structured logging with Serilog
- **RabbitMQ Management**: Available at http://localhost:15672 (guest/guest)

## Security Considerations

1. **Email Credentials**: Store SMTP credentials securely (use environment variables or Azure Key Vault)
2. **Input Validation**: The API validates required fields (To, Subject)
3. **Rate Limiting**: Consider implementing rate limiting for production use
4. **Authentication**: Add authentication/authorization as needed

## Error Handling

- Failed emails are logged with detailed error information
- RabbitMQ provides automatic retry mechanism
- Consider implementing dead letter queues for failed messages

## Performance

- **Non-blocking**: API responses are immediate (async processing)
- **Scalable**: Multiple consumer instances can process emails in parallel
- **Reliable**: RabbitMQ ensures message durability

## Development

### Adding New Features
1. Update the `SendEmailEvent` in EventBus.Messages if needed
2. Modify the `EmailRequest` model
3. Update the `EmailService` implementation
4. Add new controller endpoints if required

### Testing
Use the provided HTTP file or Swagger UI for testing endpoints.

