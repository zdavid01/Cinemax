using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PrivateSession.Hubs;
using PrivateSession.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddSingleton<IGoogleDriveService, GoogleDriveService>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
        corsBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials()
            .Build());
});

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings.GetValue<string>("SecretKey");

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
                
        ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
        ValidAudience = jwtSettings.GetSection("validAudience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
    
    options.Events= new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            var method = context.HttpContext.Request.Method;

            // Log for debugging
            Console.WriteLine($"JWT OnMessageReceived: {method} {path}, HasToken: {!string.IsNullOrEmpty(accessToken)}");

            // Support token from query string for SignalR hubs and video streaming
            if (!string.IsNullOrEmpty(accessToken) && 
                (path.StartsWithSegments("/notify") || 
                 path.StartsWithSegments("/chat-hub") ||
                 path.StartsWithSegments("/Movie/stream") ||
                 path.StartsWithSegments("/Movie/hls")))
            {
                // Read the token out of the query string
                context.Token = accessToken;
                Console.WriteLine($"✅ Token extracted from query string for {method} {path}");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ JWT Authentication Failed: {context.HttpContext.Request.Method} {context.HttpContext.Request.Path}");
            Console.WriteLine($"   Error: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapHub<ChatHub>("chat-hub");
app.MapControllers();

app.Run();

