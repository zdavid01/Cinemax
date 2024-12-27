using AutoMapper;
using Dapper;
using PaymentTest.API.Data;
using PaymentTest.API.Data.DTOs;
using PaymentTest.API.Data.DTOs.Payment;
using PaymentTest.API.Entities;

namespace PaymentTest.API.Repositories.Payment;

public class PaymentRepository : IPaymentRepository
{
    private IPaymentItemContext _itemContext;
    private IMapper _mapper;

    public PaymentRepository(IPaymentItemContext itemContext, IMapper mapper)
    {
        _itemContext = itemContext ?? throw new ArgumentNullException(nameof(itemContext));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<IEnumerable<Entities.Payment>> GetPaymentsByUsername(string username)
    {
        using var connection = _itemContext.GetConnection();
        
        var payments = await connection.QueryAsync<Entities.Payment>(
            "SELECT * FROM Payment"); 
        
        return _mapper.Map<IEnumerable<Entities.Payment>>(payments);
        
    }
}