using MediatR;
using Payment.Application.Features.Payments.Commands.DTOs;

namespace Payment.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommand : IRequest<int>
{
    //Money
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    
    //Payment
    public string BuyerId { get; set; }
    public string BuyerUsername { get; set; }
    public IEnumerable<PaymentItemDTO> PaymentItems { get; set; }
}