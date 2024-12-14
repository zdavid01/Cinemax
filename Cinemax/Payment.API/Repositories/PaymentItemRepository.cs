using AutoMapper;
using Dapper;
using Payment.API.Data;
using Payment.API.DTOs;
using Payment.API.Entities;

namespace Payment.API.Repositories;

public class PaymentItemRepository : IPaymentRepository
{
    private readonly IPaymentContext _context;
    private readonly IMapper _mapper;

    public PaymentItemRepository(IPaymentContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }


    public async Task<PaymentItemDTO?> GetPaymentItem(int paymentId)
    {
        //if something fails connection closes
        await using var connection = _context.GetConnection();

        var paymentItem = await connection.QueryFirstOrDefaultAsync<PaymentItem>(
            "SELECT * FROM Payment WHERE Id = @Id", 
            new { Id = paymentId });
        
        return _mapper.Map<PaymentItemDTO>(paymentItem);
    }


    public async Task<bool> CreatePaymentItem(CreatePaymentItemDTO paymentItem)
    {
        await using var connection = _context.GetConnection();

        var affected = await connection.ExecuteAsync(
            "INSERT INTO Payment (Moviename, MovieId, Price, Quantity) VALUES (@CreatedAt, @Moviename, @MovieId, @Price, @Quantity)",
                new { Moviename = paymentItem.MovieName, paymentItem.MovieId, paymentItem.Price, paymentItem.Quantity}
            );
        
        return affected != 0;
    }

    public async Task<bool> DeletePaymentItem(int paymentItemId)
    {
        await using var connection = _context.GetConnection();

        var affected = await connection.ExecuteAsync(
            "DELETE FROM Payment WHERE Id = @PaymentItemId",
            new { paymentItemId }
        );

        return affected != 0;
    }

    public async Task<IEnumerable<PaymentItemDTO>> GetPaymentItemsForMovie(string movieId)
    {
        await using var connection = _context.GetConnection();

        var allPaymentItems = await connection.QueryAsync<PaymentItem>(
            "SELECT * FROM Payment WHERE MovieId = @movieId",
            new { movieId });
        
        return _mapper.Map<IEnumerable<PaymentItemDTO>>(allPaymentItems);
        
    }
}