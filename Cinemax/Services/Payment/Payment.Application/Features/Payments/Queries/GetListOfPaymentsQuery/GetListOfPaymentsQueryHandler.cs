using MediatR;
using Payment.Application.Contracts.Persistence;
using Payment.Application.Factories;
using Payment.Application.Features.Payments.Queries.ViewModels;

namespace Payment.Application.Features.Payments.Queries.GetListOfPaymentsQuery;

public class GetListOfPaymentsQueryHandler : IRequestHandler<GetListOfPaymentsQuery, List<PaymentViewModel>>
{
    
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentViewModelFactory _factory;

    public GetListOfPaymentsQueryHandler(IPaymentRepository paymentRepository, IPaymentViewModelFactory factory)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }
    
    public async Task<List<PaymentViewModel>> Handle(GetListOfPaymentsQuery request, CancellationToken _)
    {
        var paymentList = await _paymentRepository.GetPaymentsByUsername(request.Username);
        return paymentList.Select(p => _factory.CreateViewModel(p)).ToList();
    }
}