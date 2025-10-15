using Email.API.Consumers;
using Email.API.Models;
using Email.API.Services;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Email Settings
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Register services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IMessageProducer, MessageProducer>();

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Add consumers
    x.AddConsumer<SendEmailConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672");

        // Configure the consumer endpoint
        cfg.ReceiveEndpoint("send-email-queue", e =>
        {
            e.ConfigureConsumer<SendEmailConsumer>(context);
            
            // Configure retry policy
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
            
            // Configure error handling
            e.UseInMemoryOutbox();
        });
    });
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting Email.API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Email.API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


