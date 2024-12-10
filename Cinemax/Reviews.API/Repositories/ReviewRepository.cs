using MongoDB.Driver;
using Reviews.API.Context;
using Reviews.API.Entities;

namespace Reviews.API.Repositories;

public class ReviewRepository : IReviewRepository
{
    
    private readonly IReviewsContext _context;

    public ReviewRepository(IReviewsContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<Review>> GetReviews()
    {
        return await _context.Reviews.Find(r=>true).ToListAsync();
    }

    public async Task<Review> GetReviewById(string id)
    {
        return await _context.Reviews.Find(r=>r.Id == id).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewByUserId(string userId)
    {
        return await _context.Reviews.Find(r => r.UserId == userId).ToListAsync();
    }

    public async Task CreateReview(Review review)
    {
        await _context.Reviews.InsertOneAsync(review);
    }

    public Task<bool> UpdateReview(Review product)
    {
        var updateResult = _context.Reviews.ReplaceOneAsync(r => r.Id == product.Id, product);
        return Task.FromResult(updateResult.IsCompleted && updateResult.Result.ModifiedCount > 0);
    }

    public Task<bool> DeleteReview(string id)
    {
        var deleteResult = _context.Reviews.DeleteOneAsync(r => r.Id == id);
        return Task.FromResult(deleteResult.IsCompleted && deleteResult.Result.DeletedCount > 0);
    }
}