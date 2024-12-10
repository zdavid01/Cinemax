using MongoDB.Driver;
using Reviews.API.Entities;

namespace Reviews.API.Context;

public class ReviewsContext : IReviewsContext
{
    public IMongoCollection<Review> Reviews { get; }

    public ReviewsContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        var database = client.GetDatabase("ReviewsDB");

        Reviews = database.GetCollection<Review>("Reviews");
    }
}