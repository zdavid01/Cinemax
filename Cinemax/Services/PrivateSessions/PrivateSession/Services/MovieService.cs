using PrivateSession.DTOs;

namespace PrivateSession.Services;

public class MovieService : IMovieService
{
    private readonly IGoogleDriveService _googleDriveService;
    private readonly ILogger<MovieService> _logger;

    public MovieService(IGoogleDriveService googleDriveService, ILogger<MovieService> logger)
    {
        _googleDriveService = googleDriveService;
        _logger = logger;
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all movies from Google Drive");
            var movies = await _googleDriveService.GetMoviesFromDriveAsync();
            _logger.LogInformation($"Successfully fetched {movies.Count()} movies");
            return movies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movies from Google Drive");
            // Return empty list on error
            return new List<Movie>();
        }
    }

    public async Task<Movie?> GetMovieById(string id)
    {
        try
        {
            _logger.LogInformation($"Fetching movie with ID: {id}");
            var movie = await _googleDriveService.GetMovieByIdAsync(id);
            
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {id} not found");
            }
            
            return movie;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching movie with ID: {id}");
            return null;
        }
    }
}