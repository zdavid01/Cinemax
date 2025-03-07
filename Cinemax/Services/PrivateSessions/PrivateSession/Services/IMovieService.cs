using PrivateSession.DTOs;

namespace PrivateSession.Services;

public interface IMovieService
{
    public Task<IEnumerable<Movie>> GetAllMoviesAsync();
    public Task<Movie> GetMovieById(string id);
}