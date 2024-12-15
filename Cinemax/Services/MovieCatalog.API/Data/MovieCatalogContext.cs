using MongoDB.Driver;
using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Data;

public class MovieCatalogContext : IMovieCatalogContext
{
    public MovieCatalogContext(IConfiguration config)
    {
        var client = new MongoClient(config.GetValue<string>("DataBaseSettings:ConnectionString"));
        var database = client.GetDatabase("CinemaxMoviesDB");

        Movies = database.GetCollection<Movie>("Movies");
        MovieCatalogContextSeed.SeedData(Movies);
    }
    public IMongoCollection<Movie> Movies { get; }
}