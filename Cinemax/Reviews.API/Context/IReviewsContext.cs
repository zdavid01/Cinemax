using MongoDB.Driver;
using Reviews.API.Entities;

namespace Reviews.API.Context;

public interface IReviewsContext
{
    IMongoCollection<Review> Reviews { get; }
}

