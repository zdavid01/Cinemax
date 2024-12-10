using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Reviews.API.Entities;

public class Review
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string UserId { get; set; }
    public string MovieId { get; set; }
    public string ReviewTest { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewedAt { get; set; }
}