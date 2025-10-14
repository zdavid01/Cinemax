using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using PrivateSession.DTOs;

namespace PrivateSession.Services;

/// <summary>
/// Service for handling Google Drive file operations and URL generation
/// Base Google Drive folder: https://drive.google.com/drive/u/0/folders/1MZlZG7ikum_H_Hizys5gvK0A-wz1SEq4
/// </summary>
public class GoogleDriveService : IGoogleDriveService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleDriveService> _logger;
    private readonly string _folderId;
    private readonly string _applicationName;
    private DriveService? _driveService;
    private static readonly string[] Scopes = { DriveService.Scope.DriveReadonly };

    public GoogleDriveService(IConfiguration configuration, ILogger<GoogleDriveService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _folderId = _configuration.GetValue<string>("GoogleDrive:FolderId") ?? "";
        _applicationName = _configuration.GetValue<string>("GoogleDrive:ApplicationName") ?? "Cinemax";
    }

    private async Task<DriveService> GetDriveServiceAsync()
    {
        if (_driveService != null)
            return _driveService;

        try
        {
            var credentialsPath = _configuration.GetValue<string>("GoogleDrive:CredentialsPath");
            
            if (string.IsNullOrEmpty(credentialsPath) || !File.Exists(credentialsPath))
            {
                _logger.LogError($"Credentials file not found at: {credentialsPath}");
                throw new FileNotFoundException("Google Drive credentials file not found");
            }

            // Use Service Account authentication (no browser needed!)
            GoogleCredential credential;
            using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(Scopes);
            }

            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });

            _logger.LogInformation("Google Drive service initialized successfully with Service Account");
            return _driveService;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Google Drive service");
            throw;
        }
    }

    public async Task<IEnumerable<Movie>> GetMoviesFromDriveAsync()
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var movies = new List<Movie>();

            var request = service.Files.List();
            request.Q = $"'{_folderId}' in parents and (mimeType contains 'video/' or mimeType='application/vnd.google-apps.folder')";
            request.Fields = "files(id, name, mimeType, description, createdTime, thumbnailLink, videoMediaMetadata)";
            request.PageSize = 100;

            var result = await request.ExecuteAsync();

            if (result.Files != null)
            {
                foreach (var file in result.Files)
                {
                    // Only process video files
                    if (file.MimeType != null && file.MimeType.Contains("video"))
                    {
                        var movie = new Movie
                        {
                            Id = file.Id,
                            Title = Path.GetFileNameWithoutExtension(file.Name),
                            Description = file.Description ?? "No description available",
                            ImageUrl = file.ThumbnailLink ?? GetImageUrl(file.Id),
                            ReleaseDate = file.CreatedTime ?? DateTime.Now,
                            ExpiresInDays = 30
                        };
                        movies.Add(movie);
                        _logger.LogInformation($"Found video: {file.Name} (ID: {file.Id})");
                    }
                }
            }

            _logger.LogInformation($"Found {movies.Count} videos in Google Drive folder");
            return movies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movies from Google Drive");
            throw;
        }
    }

    public async Task<Movie?> GetMovieByIdAsync(string fileId)
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var request = service.Files.Get(fileId);
            request.Fields = "id, name, mimeType, description, createdTime, thumbnailLink, videoMediaMetadata";

            var file = await request.ExecuteAsync();

            if (file != null && file.MimeType != null && file.MimeType.Contains("video"))
            {
                return new Movie
                {
                    Id = file.Id,
                    Title = Path.GetFileNameWithoutExtension(file.Name),
                    Description = file.Description ?? "No description available",
                    ImageUrl = file.ThumbnailLink ?? GetImageUrl(file.Id),
                    ReleaseDate = file.CreatedTime ?? DateTime.Now,
                    ExpiresInDays = 30
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching movie with ID: {fileId}");
            return null;
        }
    }

    public string GetDirectDownloadUrl(string fileId)
    {
        // Google Drive direct download URL for files
        return $"https://drive.google.com/uc?export=download&id={fileId}";
    }

    public string GetImageUrl(string fileId)
    {
        // Google Drive thumbnail URL
        return $"https://drive.google.com/thumbnail?id={fileId}&sz=w400";
    }

    public string GetVideoStreamUrl(string fileId)
    {
        // For video streaming, we can use the webContentLink or construct a streaming URL
        // Google Drive supports direct video streaming via this URL format
        return $"https://drive.google.com/uc?export=download&id={fileId}";
    }

    public async Task<Stream> GetFileStreamAsync(string fileId)
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var request = service.Files.Get(fileId);
            
            // Use a MemoryStream for smaller files
            // For production, consider using a FileStream or streaming directly
            var stream = new MemoryStream();
            
            _logger.LogInformation($"Starting download of file {fileId}");
            
            // Download the file
            var progress = await request.DownloadAsync(stream);
            
            if (progress.Status == Google.Apis.Download.DownloadStatus.Completed)
            {
                stream.Position = 0;
                _logger.LogInformation($"Successfully downloaded {progress.BytesDownloaded} bytes for fileId: {fileId}");
                return stream;
            }
            else
            {
                _logger.LogError($"Download failed with status: {progress.Status}");
                if (progress.Exception != null)
                {
                    _logger.LogError(progress.Exception, "Download exception");
                }
                throw new Exception($"Failed to download file: {progress.Status}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching file stream for fileId: {fileId}");
            throw;
        }
    }
}
