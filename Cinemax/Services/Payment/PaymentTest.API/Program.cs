using System.Reflection;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using PaymentTest.API.Extensions;
using PaymentTest.API.Services.Email.Contracts;
using PaymentTest.API.Services.Email.Persistance;
using PaymentTest.API.Services.Email.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//env
Env.Load();
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


//mail
builder.Services.Configure<GmailOptions>(
    builder.Configuration.GetSection(GmailOptions.GmailOptionsKey)
);

builder.Services.AddScoped<IMailService, GmailService>();

//controller
builder.Services.AddControllers();

//paymentItem
builder.Services.AddPaymentItemServices();

//payment
// builder.Services.AddPaymentServices();


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

app.MapPost("/email", async ([FromBody] SendEmailRequest sendEmailRequest, IMailService mailService) =>
    {
        await mailService.SendEmailAsync(sendEmailRequest);
        
        return Results.Ok("Email sent successfully");
    } 
);

app.Run();