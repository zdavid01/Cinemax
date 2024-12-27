using PaymentTest.API.Data.DTOs.Payment;
using PaymentTest.API.Data.Payment;
using PaymentTest.API.Entities;
using PaymentTest.API.Repositories.Payment;

namespace PaymentTest.API.Extensions;

public static class PaymentExtension
{
    public static void AddPaymentServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentContext, PaymentContext>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddAutoMapper(configuration =>
        {
            configuration.CreateMap<PaymentDTO, Payment>().ReverseMap();
        });
    }
}