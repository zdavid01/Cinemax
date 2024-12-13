using MongoDB.Driver;
using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Data;

public class MovieCatalogContextSeed
{
    public static void SeedData(IMongoCollection<Movie> moviesCollection)
    {
        var existMovies = moviesCollection.Find(p => true).Any();
        if (!existMovies)
        {
            moviesCollection.InsertManyAsync(GetConfiguredMovies());
        }
    }

    private static IEnumerable<Movie> GetConfiguredMovies()
    {
        return new List<Movie>()
        {
            new Movie()
            {
                Id = "602d2149e773f2a3990b47f5",
                Title = "Star Wars: The Force Awakens",
                Description = "The Force Awakens was the future of the world.",
                Genre = "Action",
                Actors = "The Force Awakens",
                Director = "J. K. Rowling",
                Length = 120
            },
            new Movie()
            {
                Id = "602d2149e773f2a3990b47f6",
                Title = "Harry Potter",
                Description = "The Force Awakens was the future of the world.",
                Genre = "Action",
                Actors = "The Force Awakens",
                Director = "J. K. Rowling",
                Length = 120
            }
        };
    }
}