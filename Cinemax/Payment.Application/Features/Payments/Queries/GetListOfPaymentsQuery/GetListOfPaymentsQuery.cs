using MediatR;
using Payment.Application.Features.Payments.Queries.ViewModels;

namespace Payment.Application.Features.Payments.Queries.GetListOfPaymentsQuery;

public class GetListOfPaymentsQuery : IRequest<List<PaymentViewModel>>
{
    public string Username { get; set; }

    public GetListOfPaymentsQuery(string username)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
    }
}