using PrivateSession.DTOs;

namespace PrivateSession.Services;

public interface IMovieService
{
    public Task<IEnumerable<Movie>> GetAllMoviesAsync(string? username = null);
    public Task<Movie?> GetMovieById(string id, string? username = null);
}