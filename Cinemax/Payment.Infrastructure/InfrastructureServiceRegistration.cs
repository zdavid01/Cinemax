using Microsoft.EntityFrameworkCore;
using Payment.Application.Contracts.Infrastructure;
using Payment.Application.Contracts.Persistence;
using Payment.Application.Factories;
using Payment.Application.Models;
using Payment.Infrastructure.Factories;
using Payment.Infrastructure.Mail;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Repositories;

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
        
        services.Configure<EmailSettings>(c =>
        {
            var config = configuration.GetSection("EmailSettings");
            c.Mail = config["Mail"];
            c.DisplayName = config["DisplayName"];
            c.Password = config["Password"];
            c.Host = config["Host"];
            c.Port = int.Parse(config["Port"]);
        });
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}