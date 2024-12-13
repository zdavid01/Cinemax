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

    public async Task<Movie> GetMovieById(string id)
    {
        return await _context.Movies.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Movie>> GetMoviesByGenre(string genre)
    {
        return await _context.Movies.Find(p => p.Genre == genre).ToListAsync();
    }

    public async Task CreateMovie(Movie movie)
    {
        await _context.Movies.InsertOneAsync(movie);
    }

    public async Task<bool> UpdateMovie(Movie movie)
    {
        var updateRes = await _context.Movies.ReplaceOneAsync(p => p.Id == movie.Id, movie);
        return updateRes.IsAcknowledged && updateRes.ModifiedCount > 0;
    }

    public async Task<bool> DeleteMovie(string id)
    {
        var deleteRes = await _context.Movies.DeleteOneAsync(p => p.Id == id);
        return deleteRes.IsAcknowledged && deleteRes.DeletedCount > 0;
    }
}