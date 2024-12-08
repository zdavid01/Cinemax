using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetMovies();
}