using MongoDB.Driver;
using Reviews.API.Entities;

namespace Reviews.API.Data;

public class ReviewDataSeed
{
    public static void SeedData(IMongoCollection<Review> reviewCollection)
    {
        var existsReviews = reviewCollection.Find(r => true).Any();
        
        if (!existsReviews)
        {
            reviewCollection.InsertManyAsync((GetSampleReviews()));    
        }
    }

    private static IEnumerable<Review> GetSampleReviews()
    {
        return new List<Review>()
        {
            new Review()
            {
                Id = "602d2149e773f2a3990b47f5",
                UserId = "602d2149e773f2a3990u47f5",
                MovieId = "602d2149e773f2a3990m47f5",
                ReviewTest = "Great movie.",
                Rating = 5,
                ReviewedAt = DateTime.Now,
            },
            
            new Review()
            {
                Id = "602d3149e773f2a3990b47f5",
                UserId = "603d2149e773f2a3990u47f5",
                MovieId = "603d2149e773f2a3990m47f5",
                ReviewTest = "Good movie.",
                Rating = 4,
                ReviewedAt = DateTime.Now - TimeSpan.FromDays(30),
            },
            
            new Review()
            {
                Id = "604d2149e773f2a3990b47f5",
                UserId = "604d2149e773f2a3990u47f5",
                MovieId = "604d2149e773f2a3990m47f5",
                ReviewTest = "Average movie.",
                Rating = 3,
                ReviewedAt = DateTime.Now - TimeSpan.FromDays(20),
            },
            
            new Review()
            {
                Id = "605d2149e773f2a3990b47f5",
                UserId = "602d2149e773f2a3990u47f5",
                MovieId = "605d2149e773f2a3990m47f5",
                ReviewTest = "Great movie. Recommended.",
                Rating = 5,
                ReviewedAt = DateTime.Now - TimeSpan.FromDays(10),
            },
        };
    }
}