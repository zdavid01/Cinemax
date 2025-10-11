using System.Reflection;
using System.Text;
using Payment.Application;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence;
using Payment.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to support both HTTP/1.1 (REST) and HTTP/2 (gRPC) on port 8080
builder.WebHost.ConfigureKestrel(options =>
{
    // Listen on port 8080 for both HTTP/1.1 and HTTP/2 - unencrypted for internal service communication
    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add gRPC
builder.Services.AddGrpc();

// Add Redis connection for accessing basket data (with resilient connection)
var redisConnectionString = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString") ?? "basketdb:6379";
builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var logger = sp.GetRequiredService<ILogger<Program>>();
    try 
    {
        var configOptions = ConfigurationOptions.Parse(redisConnectionString);
        configOptions.AbortOnConnectFail = false; // Don't crash if Redis isn't immediately available
        configOptions.ConnectRetry = 5;
        configOptions.ConnectTimeout = 5000;
        
        var multiplexer = ConnectionMultiplexer.Connect(configOptions);
        logger.LogInformation("Successfully connected to Redis at {ConnectionString}", redisConnectionString);
        return multiplexer;
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Failed to connect to Redis at {ConnectionString}. Payment API will continue without Redis basket integration.", redisConnectionString);
        // Return a disconnected multiplexer - the app will still start but basket retrieval will fail gracefully
        return ConnectionMultiplexer.Connect($"{redisConnectionString},abortConnect=false");
    }
});

//
builder.Services.AddControllers();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

//Mapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        
        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
        ValidAudience = jwtSettings.GetSection("validAudience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// CORS for Angular dev server
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaCors", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("SpaCors");
app.UseAuthentication();
app.UseAuthorization();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PaymentContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<PaymentContext>>();
    
    try
    {
        // Create database if it doesn't exist
        await context.Database.EnsureCreatedAsync();
        
        // Seed the database
        await PaymentContextSeed.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

// HTTP redirect is disabled for containerized environment
// app.UseHttpsRedirection();

app.MapControllers();
app.MapGrpcService<PaymentGrpcService>();
app.Run();
