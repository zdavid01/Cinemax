using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PrivateSession.Services;

namespace PrivateSession.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IGoogleDriveService _googleDriveService;
    private readonly ILogger<MovieController> _logger;
    private readonly IConfiguration _configuration;

    public MovieController(IMovieService movieService, IGoogleDriveService googleDriveService, ILogger<MovieController> logger, IConfiguration configuration)
    {
        _movieService = movieService;
        _googleDriveService = googleDriveService;
        _logger = logger;
        _configuration = configuration;
    }

    [Authorize]
    [HttpGet("movies")]
    public async Task<IActionResult> GetMovies()
    {
        // Get username from JWT claims
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("Could not extract username from JWT token");
            return Unauthorized("Invalid token");
        }

        _logger.LogInformation($"Fetching movies for user: {username}");
        
        // Filter movies based on user's purchases
        var movies = await _movieService.GetAllMoviesAsync(username);
        return Ok(movies);
    }
    
    [HttpGet("imageForMovie/{fileId}")]
    public async Task<IActionResult> GetImageForMovie(string fileId)
    {
        _logger.LogInformation($"Returning image for file {fileId}");
        try
        {
            var imageUrl = _googleDriveService.GetImageUrl(fileId);
            _logger.LogInformation($"Redirecting to Google Drive image URL: {imageUrl}");
            return Redirect(imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting image for file {fileId}");
            return NotFound();
        }
    }
    
    [Authorize]
    [HttpGet("{movieId}")]
    public async Task<ActionResult> Get(string movieId)
    {
        // Get username from JWT claims
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        
        if (string.IsNullOrEmpty(username))
        {
            _logger.LogWarning("Could not extract username from JWT token");
            return Unauthorized("Invalid token");
        }

        var movie = await _movieService.GetMovieById(movieId, username);
        if (movie == null)
        {
            return NotFound("Movie not found or you don't have access to it");
        }
        return Ok(movie);
    }

    [Authorize]
    [HttpGet("stream/{fileId}")]
    public async Task<ActionResult> StreamMovie(string fileId)
    {
        try
        {
            // Get username from JWT claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                _logger.LogWarning("Could not extract username from JWT token");
                return Unauthorized("Invalid token");
            }

            // Verify user has purchased this movie
            var movie = await _movieService.GetMovieById(fileId, username);
            if (movie == null)
            {
                _logger.LogWarning($"User {username} does not have access to movie {fileId}");
                return Forbid("You don't have access to this movie. Please purchase it first.");
            }

            _logger.LogInformation($"Proxying video stream for file {fileId} for user {username}");
            
            // Get the video stream from Google Drive
            var stream = await _googleDriveService.GetFileStreamAsync(fileId);
            
            if (stream == null)
            {
                _logger.LogWarning($"Video stream not found for file {fileId}");
                return NotFound();
            }

            // Return the stream with proper content type
            // Let the browser determine content type, or set it to video/mp4 as default
            return File(stream, "video/mp4", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error streaming video {fileId}");
            return StatusCode(500, "Error streaming video");
        }
    }
}