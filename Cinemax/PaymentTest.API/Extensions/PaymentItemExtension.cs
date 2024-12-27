using PaymentTest.API.Data;
using PaymentTest.API.Data.DTOs;
using PaymentTest.API.Entities;
using PaymentTest.API.Repositories;

namespace PaymentTest.API.Extensions;

public static class PaymentItemExtension
{
    public static void AddPaymentItemServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentItemContext, PaymentItemContext>();
        services.AddScoped<IPaymentItemRepository, PaymentItemRepository>();

        services.AddAutoMapper(configuration =>
        {
            configuration.CreateMap<PaymentItemDTO, PaymentItem>().ReverseMap();
        });
    }
}