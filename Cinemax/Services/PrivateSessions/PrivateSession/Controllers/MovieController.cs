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
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
        {
            return NotFound();
        }
        return Ok(movie);
    }

    [Authorize]
    [HttpGet("stream/{fileId}")]
    public async Task<ActionResult> StreamMovie(string fileId)
    {
        try
        {
            _logger.LogInformation($"Proxying video stream for file {fileId}");
            
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