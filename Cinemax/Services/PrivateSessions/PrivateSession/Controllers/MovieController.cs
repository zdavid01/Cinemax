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
    [HttpGet("stream/{fileOrFolderId}")]
    public async Task<ActionResult> StreamMovie(string fileOrFolderId)
    {
        try
        {
            // Get username from JWT claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid token");
            }

            // Verify user has purchased this movie
            var movie = await _movieService.GetMovieById(fileOrFolderId, username);
            if (movie == null)
            {
                _logger.LogWarning($"User {username} does not have access to movie {fileOrFolderId}");
                return Forbid("You don't have access to this movie. Please purchase it first.");
            }

            // Check if it's a folder (HLS) or file (direct stream)
            var playlistId = await _googleDriveService.GetPlaylistFileIdAsync(fileOrFolderId);
            
            if (!string.IsNullOrEmpty(playlistId))
            {
                // It's a folder with HLS - redirect to HLS endpoint
                // Pass through the access token if provided
                var accessToken = Request.Query["access_token"].ToString();
                var redirectUrl = $"/Movie/hls/{fileOrFolderId}/playlist.m3u8";
                
                if (!string.IsNullOrEmpty(accessToken))
                {
                    redirectUrl += $"?access_token={accessToken}";
                }
                
                _logger.LogInformation($"HLS folder detected, redirecting to playlist: {redirectUrl}");
                return Redirect(redirectUrl);
            }
            else
            {
                // It's a single video file - stream it directly
                _logger.LogInformation($"Single file detected, streaming directly for {fileOrFolderId}");
                var stream = await _googleDriveService.GetFileStreamAsync(fileOrFolderId);
                
                if (stream == null)
                {
                    return NotFound();
                }

                return File(stream, "video/mp4", enableRangeProcessing: true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error streaming {fileOrFolderId}");
            return StatusCode(500, "Error streaming video");
        }
    }

    [Authorize]
    [HttpGet("hls/{folderId}/playlist.m3u8")]
    [HttpHead("hls/{folderId}/playlist.m3u8")]
    public async Task<ActionResult> GetPlaylist(string folderId)
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
            var movie = await _movieService.GetMovieById(folderId, username);
            if (movie == null)
            {
                _logger.LogWarning($"User {username} does not have access to movie folder {folderId}");
                return Forbid("You don't have access to this movie. Please purchase it first.");
            }
            
            // Get the playlist file ID from the folder
            var playlistFileId = await _googleDriveService.GetPlaylistFileIdAsync(folderId);
            
            if (string.IsNullOrEmpty(playlistFileId))
            {
                _logger.LogWarning($"Playlist file not found in folder {folderId}");
                return NotFound("Playlist file not found");
            }

            // For HEAD requests, just return 200 OK
            if (Request.Method == "HEAD")
            {
                return Ok();
            }

            _logger.LogInformation($"Serving playlist for folder {folderId} to user {username}");

            // Download and serve the playlist
            var playlistStream = await _googleDriveService.GetFileStreamAsync(playlistFileId);
            
            if (playlistStream == null)
            {
                return NotFound("Failed to load playlist");
            }

            // Read the playlist content and update chunk URLs to point to our API
            using var reader = new StreamReader(playlistStream);
            var playlistContent = await reader.ReadToEndAsync();
            
            // Get access token to pass to chunks
            var accessToken = Request.Query["access_token"].ToString();
            
            // Replace relative chunk URLs with our API endpoints
            var modifiedPlaylist = ModifyPlaylistUrls(playlistContent, folderId, accessToken);
            
            return Content(modifiedPlaylist, "application/vnd.apple.mpegurl");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error serving playlist for folder {folderId}");
            return StatusCode(500, "Error loading playlist");
        }
    }

    [Authorize]
    [HttpGet("hls/{folderId}/{chunkFileName}")]
    public async Task<ActionResult> GetChunk(string folderId, string chunkFileName)
    {
        try
        {
            // Get username from JWT claims
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            // Verify user has purchased this movie
            var movie = await _movieService.GetMovieById(folderId, username);
            if (movie == null)
            {
                return Forbid();
            }

            _logger.LogInformation($"Serving chunk {chunkFileName} from folder {folderId}");
            
            // Find the chunk file in the folder
            var chunkFileId = await _googleDriveService.FindFileInFolderAsync(folderId, chunkFileName);
            
            if (string.IsNullOrEmpty(chunkFileId))
            {
                _logger.LogWarning($"Chunk file {chunkFileName} not found in folder {folderId}");
                return NotFound();
            }

            // Stream the chunk file
            var stream = await _googleDriveService.GetFileStreamAsync(chunkFileId);
            
            if (stream == null)
            {
                return NotFound();
            }

            // Return with proper MIME type for HLS chunks
            return File(stream, "video/mp2t", enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error serving chunk {chunkFileName} from folder {folderId}");
            return StatusCode(500, "Error loading video chunk");
        }
    }

    private string ModifyPlaylistUrls(string playlistContent, string folderId, string? accessToken)
    {
        // Replace relative .ts file references with full API URLs
        var lines = playlistContent.Split('\n');
        var modifiedLines = new List<string>();

        foreach (var line in lines)
        {
            if (line.Trim().EndsWith(".ts"))
            {
                // Extract just the filename
                var fileName = Path.GetFileName(line.Trim());
                // Replace with full URL to our API endpoint
                var chunkUrl = $"/Movie/hls/{folderId}/{fileName}";
                
                // Add access token if provided
                if (!string.IsNullOrEmpty(accessToken))
                {
                    chunkUrl += $"?access_token={accessToken}";
                }
                
                modifiedLines.Add(chunkUrl);
            }
            else
            {
                modifiedLines.Add(line);
            }
        }

        return string.Join("\n", modifiedLines);
    }
}