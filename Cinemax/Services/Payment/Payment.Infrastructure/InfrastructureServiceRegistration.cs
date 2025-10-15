using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Contracts.Persistence;
using Payment.Application.Factories;
using Payment.Application.Models;
using Payment.Infrastructure.Factories;
using Payment.Infrastructure.Mail;
using Payment.Infrastructure.PayPal;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Repositories;
using Payment.Infrastructure.Services;

namespace Payment.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PaymentConnectionString") ?? 
                             configuration["DatabaseSettings:ConnectionSettings"]));

        services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        
        services.AddScoped<IPaymentFactory, PaymentFactory>();
        services.AddScoped<IPaymentViewModelFactory, PaymentViewModelFactory>();
        
        // Register HttpClient for Email.API
        services.AddHttpClient<IEmailService, EmailApiClient>();
        
        // Register Basket Service
        services.AddScoped<IBasketService, BasketService>();
            
        // Register PayPal service
        services.AddScoped<PayPalService>();

        return services;
    }
}