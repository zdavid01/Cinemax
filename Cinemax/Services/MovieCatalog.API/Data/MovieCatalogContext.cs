using MongoDB.Driver;
using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Data;

public class MovieCatalogContext : IMovieCatalogContext
{
    public MovieCatalogContext()
    {
        var client = new MongoClient("mongodb://localhost:27017");
        var database = client.GetDatabase("CinemaxMoviesDB");

        Movies = database.GetCollection<Movie>("Movies");
        MovieCatalogContextSeed.SeedData(Movies);
    }
    public IMongoCollection<Movie> Movies { get; }
}