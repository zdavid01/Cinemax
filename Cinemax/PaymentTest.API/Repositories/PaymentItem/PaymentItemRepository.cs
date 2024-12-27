using AutoMapper;
using Dapper;
using PaymentTest.API.Data;
using PaymentTest.API.Data.DTOs;
using PaymentTest.API.Entities;

namespace PaymentTest.API.Repositories;

public class PaymentItemRepository : IPaymentItemRepository
{
    private readonly IPaymentContext _context;

    private readonly IMapper _mapper;

    public PaymentItemRepository(IPaymentContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public async Task<List<PaymentItemDTO>> GetPaymentItems()
    {
        using var connection = _context.GetConnection();
        
        var payments = await connection.QueryAsync<PaymentItem>(
            "SELECT * FROM PaymentItem"); 
        
        return _mapper.Map<List<PaymentItemDTO>>(payments);
        
    }

    public async Task<PaymentItemDTO> GetPaymentItemByMovieId(string movieId)
    {
        using var connection = _context.GetConnection();

        var paymentItem = await connection.QueryFirstOrDefaultAsync<PaymentItem>(
            "SELECT * FROM PaymentItem WHERE  MovieId = @movieId", new { movieId });
        
        return _mapper.Map<PaymentItemDTO>(paymentItem);
    }

    public async Task<bool> CreatePaymentItem(CreatePaymentItemDTO paymentItem)
    {
        using var connection = _context.GetConnection();

        var affected = await connection.ExecuteAsync(
            "INSERT INTO PaymentItem (createdBy, userId, movieName, movieId, price, quantity) VALUES ('admin', @userId, @movieName, @movieId, @price, @quantity)",
            new {userId = paymentItem.UserId, movieName = paymentItem.MovieName, movieId = paymentItem.MovieId, price = paymentItem.Price, quantity = paymentItem.Quantity});
        
        return affected >= 1;
    }

    public async Task<bool> UpdatePaymentItem(UpdatePaymentItemDTO paymentItem)
    {
        await using var connection = _context.GetConnection();

        /*, MovieName = @MovieName, MovieId = @MovieId, Price = @Price, Quantity = @Quantity*/
        // , paymentItem.MovieName, paymentItem.Price, paymentItem.Quantity, paymentItem.Id
        var affected = await connection.ExecuteAsync(
            "UPDATE PaymentItem SET UserId = @UserId, MovieName = @MovieName, MovieId = @MovieId, Price = @Price, Quantity = @Quantity WHERE Id = @Id",
            new { paymentItem.UserId, paymentItem.MovieName, paymentItem.MovieId, paymentItem.Price, paymentItem.Quantity, paymentItem.Id });

        if (affected == 0)
            return false;

        return true;
    }

    public async Task<bool> DeletePaymentItem(int id)
    {
        using var connection = _context.GetConnection();

        var affected = await connection.ExecuteAsync(
            "DELETE FROM PaymentItem WHERE Id = @id",
            new { id });

        if (affected == 0)
            return false;

        return true;
    }

    public async Task<IEnumerable<PaymentItemDTO>> GetPaymentItemsByMovieId(string movieId)
    {
        using var connection = _context.GetConnection();
        
        var payments = await connection.QueryAsync<PaymentItem>(
            "SELECT * FROM PaymentItem WHERE MovieId = @movieId", new {movieId}); 
        
        return _mapper.Map<IEnumerable<PaymentItemDTO>>(payments);
    }

    public async Task<IEnumerable<PaymentItemDTO>> GetPaymentItemsByUserId(string userId)
    {
        using var connection = _context.GetConnection();
        
        var payments = await connection.QueryAsync<PaymentItem>(
            "SELECT * FROM PaymentItem WHERE UserId = @userId", new {userId}); 
        
        return _mapper.Map<IEnumerable<PaymentItemDTO>>(payments);
    }
}