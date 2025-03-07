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
    private readonly ILogger<MovieController> _logger;
    private readonly IConfiguration _configuration;

    private readonly String baseLocation;

    public MovieController(IMovieService movieService, ILogger<MovieController> logger, IConfiguration configuration)
    {
        _movieService = movieService;
        _logger = logger;
        _configuration = configuration;
        
        this.baseLocation = _configuration.GetValue<String>("MoviesFolder");
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
        StringBuilder sb = new StringBuilder().Append(this.baseLocation).Append(movieId).Append("/image.jpg");
        return PhysicalFile( sb.ToString(), "application/octet-stream");
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
        StringBuilder sb = new StringBuilder().Append(this.baseLocation).Append(movieId).Append("/").Append(filename);

        return PhysicalFile(sb.ToString(), "application/x-mpegURL");
    }
    
    [Authorize]
    [HttpGet("stream/{movieId}")]
    public async Task<ActionResult> StreamMovie(string movieId)
    {
        var movie = await _movieService.GetMovieById(movieId);
        if (movie == null)
            return NotFound();

        // Streaming logic (serve HLS or DASH)
        _logger.LogInformation($"Streaming movie for {movieId}");
        // return PhysicalFile(movie.StreamUrl, "application/octet-stream");
        
        StringBuilder sb = new StringBuilder().Append(this.baseLocation).Append(movieId).Append("/output.m3u8");

        return PhysicalFile(sb.ToString(), "application/x-mpegURL");
    }
}