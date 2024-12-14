using Payment.API.Data;
using Payment.API.DTOs;
using Payment.API.Entities;
using Payment.API.Repositories;
namespace Payment.API.Extension;

using Microsoft.Extensions.DependencyInjection;

public static class PaymentExtension
{
    public static void AddPaymentServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentContext, PaymentContext>();
        services.AddScoped<IPaymentRepository, PaymentItemRepository>();
        services.AddAutoMapper(configuration =>
        {
            configuration.CreateMap<PaymentItemDTO, PaymentItem>().ReverseMap();
        });
        
        
        
        //todo FIX this
    }
}