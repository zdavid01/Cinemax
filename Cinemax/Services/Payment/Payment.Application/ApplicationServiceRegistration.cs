using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Payment.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //todo add validators
        });

        return services;
    }
}