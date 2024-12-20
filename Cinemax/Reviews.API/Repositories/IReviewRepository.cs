using Reviews.API.Entities;

namespace Reviews.API.Repositories;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetReviews ();
    Task<Review> GetReviewById(string id);
    Task<IEnumerable<Review>> GetReviewByUserId(string userId);
    Task<IEnumerable<Review>> GetReviewsByMovieId(string movieId);
    Task CreateReview(Review review);
    Task<bool> UpdateReview(Review review);
    Task<bool> DeleteReview(string id);
    
}