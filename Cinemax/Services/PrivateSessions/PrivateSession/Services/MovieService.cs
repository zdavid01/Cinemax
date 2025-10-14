using PrivateSession.DTOs;

namespace PrivateSession.Services;

public class MovieService : IMovieService
{
    private readonly IGoogleDriveService _googleDriveService;
    private readonly IPaymentService _paymentService;
    private readonly ILogger<MovieService> _logger;

    public MovieService(
        IGoogleDriveService googleDriveService, 
        IPaymentService paymentService,
        ILogger<MovieService> logger)
    {
        _googleDriveService = googleDriveService;
        _paymentService = paymentService;
        _logger = logger;
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync(string? username = null)
    {
        try
        {
            _logger.LogInformation("Fetching all movies from Google Drive");
            var movies = await _googleDriveService.GetMoviesFromDriveAsync();
            
            // If username provided, filter by purchased movies
            if (!string.IsNullOrEmpty(username))
            {
                _logger.LogInformation($"Filtering movies for user: {username}");
                var purchasedMovieNames = await _paymentService.GetPurchasedMovieNamesAsync(username);
                
                if (purchasedMovieNames.Any())
                {
                    // Filter movies to only those purchased
                    movies = movies.Where(m => purchasedMovieNames.Contains(m.Title, StringComparer.OrdinalIgnoreCase));
                    _logger.LogInformation($"User {username} has access to {movies.Count()} movies");
                }
                else
                {
                    _logger.LogInformation($"User {username} has no purchased movies");
                    return new List<Movie>();
                }
            }
            
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

    public async Task<Movie?> GetMovieById(string id, string? username = null)
    {
        try
        {
            _logger.LogInformation($"Fetching movie with ID: {id}");
            var movie = await _googleDriveService.GetMovieByIdAsync(id);
            
            if (movie == null)
            {
                _logger.LogWarning($"Movie with ID {id} not found");
                return null;
            }
            
            // If username provided, verify they purchased this movie
            if (!string.IsNullOrEmpty(username))
            {
                var purchasedMovieNames = await _paymentService.GetPurchasedMovieNamesAsync(username);
                
                if (!purchasedMovieNames.Contains(movie.Title, StringComparer.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"User {username} has not purchased movie: {movie.Title}");
                    return null;
                }
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