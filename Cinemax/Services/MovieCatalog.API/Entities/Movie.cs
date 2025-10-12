using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieCatalog.API.Entities;

public class Movie
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }
    public int Length { get; set; }
    public string Genre { get; set; }
    public string Director { get; set; }
    public string Actors { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string linkToTrailer { get; set; }
    public string Rating { get; set; }
    public decimal Price { get; set; }
}