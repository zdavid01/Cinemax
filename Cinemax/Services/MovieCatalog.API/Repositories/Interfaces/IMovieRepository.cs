using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetMovies();
    
    Task<Movie> GetMovieById(string id);
    
    Task<IEnumerable<Movie>> GetMoviesByGenre(string genre);
    
    Task CreateMovie(Movie movie);
    
    Task<bool> UpdateMovie(Movie movie);
    
    Task<bool> DeleteMovie(string id);
}