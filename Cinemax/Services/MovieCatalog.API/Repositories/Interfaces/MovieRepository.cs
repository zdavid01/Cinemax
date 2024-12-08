using MovieCatalog.API.Data;
using MovieCatalog.API.Entities;
using MongoDB.Driver;

namespace MovieCatalog.API.Repositories.Interfaces;

public class MovieRepository : IMovieRepository
{
    private readonly IMovieCatalogContext _context;

    public MovieRepository(IMovieCatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Movie>> GetMovies()
    {
        return await _context.Movies.Find(p => true).ToListAsync();
    }
}