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
        // add filtering so that only movies for specific user got returned
        var movies = await _movieService.GetAllMoviesAsync();
        return Ok(movies);
    }
    
    [HttpGet("imageForMovie/{movieId}")]
    public async Task<IActionResult> GetImageForMovie(string movieId)
    {
        _logger.LogInformation($"Returning image for movie {movieId}");
        try
        {
            var imageUrl = _googleDriveService.GetImageUrl(movieId);
            _logger.LogInformation($"Redirecting to Google Drive image URL: {imageUrl}");
            return Redirect(imageUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting image for movie {movieId}");
            return NotFound();
        }
    }
    
    [Authorize]
    [HttpGet("{movieId}")]
    public async Task<ActionResult> Get(string movieId)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
        {
            return NotFound();
        }
        return Ok(movie);
    }

    [Authorize]
    [HttpGet("stream/{movieId}/{filename}")]
    public async Task<IActionResult> Stream(string movieId, string filename)
    {
        try
        {
            var streamUrl = _googleDriveService.GetVideoStreamUrl(movieId, filename);
            _logger.LogInformation($"Redirecting to Google Drive stream URL: {streamUrl}");
            return Redirect(streamUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting stream for movie {movieId}, filename {filename}");
            return NotFound();
        }
    }
    
    [Authorize]
    [HttpGet("stream/{movieId}")]
    public async Task<ActionResult> StreamMovie(string movieId)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
            return NotFound();

        try
        {
            // Streaming logic (serve HLS or DASH)
            _logger.LogInformation($"Streaming movie for {movieId}");
            var streamUrl = _googleDriveService.GetVideoStreamUrl(movieId);
            _logger.LogInformation($"Redirecting to Google Drive stream URL: {streamUrl}");
            return Redirect(streamUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error streaming movie {movieId}");
            return NotFound();
        }
    }
}