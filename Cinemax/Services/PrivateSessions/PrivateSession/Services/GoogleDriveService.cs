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
            // Look for both folders (HLS) and video files (single file streaming)
            request.Q = $"'{_folderId}' in parents and (mimeType='application/vnd.google-apps.folder' or mimeType contains 'video/') and trashed=false";
            request.Fields = "files(id, name, mimeType, description, createdTime, modifiedTime, thumbnailLink)";
            request.PageSize = 100;

            var result = await request.ExecuteAsync();

            if (result.Files != null)
            {
                foreach (var file in result.Files)
                {
                    // Case 1: Folder with HLS chunks (preferred)
                    if (file.MimeType == "application/vnd.google-apps.folder")
                    {
                        var hasPlaylist = await HasPlaylistFileAsync(file.Id);
                        
                        if (hasPlaylist)
                        {
                            var movie = new Movie
                            {
                                Id = file.Id,
                                Title = file.Name,
                                Description = file.Description ?? "No description available",
                                ImageUrl = GetImageUrl(file.Id),
                                ReleaseDate = file.CreatedTime ?? DateTime.Now,
                                ExpiresInDays = 30
                            };
                            movies.Add(movie);
                            _logger.LogInformation($"Found HLS movie folder: {file.Name} (ID: {file.Id})");
                        }
                    }
                    // Case 2: Single video file (backward compatibility)
                    else if (file.MimeType != null && file.MimeType.Contains("video"))
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
                        _logger.LogInformation($"Found single video file: {file.Name} (ID: {file.Id})");
                    }
                }
            }

            _logger.LogInformation($"Found {movies.Count} movies in Google Drive (folders + files)");
            return movies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movies from Google Drive");
            throw;
        }
    }
    
    private async Task<bool> HasPlaylistFileAsync(string folderId)
    {
        try
        {
            var playlistId = await GetPlaylistFileIdAsync(folderId);
            return !string.IsNullOrEmpty(playlistId);
        }
        catch
        {
            return false;
        }
    }

    public async Task<Movie?> GetMovieByIdAsync(string fileOrFolderId)
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var request = service.Files.Get(fileOrFolderId);
            request.Fields = "id, name, mimeType, description, createdTime, thumbnailLink";

            var item = await request.ExecuteAsync();

            if (item == null)
                return null;

            // Case 1: Folder with HLS chunks
            if (item.MimeType == "application/vnd.google-apps.folder")
            {
                var hasPlaylist = await HasPlaylistFileAsync(item.Id);
                
                if (hasPlaylist)
                {
                    return new Movie
                    {
                        Id = item.Id,
                        Title = item.Name,
                        Description = item.Description ?? "No description available",
                        ImageUrl = GetImageUrl(item.Id),
                        ReleaseDate = item.CreatedTime ?? DateTime.Now,
                        ExpiresInDays = 30
                    };
                }
            }
            // Case 2: Single video file
            else if (item.MimeType != null && item.MimeType.Contains("video"))
            {
                return new Movie
                {
                    Id = item.Id,
                    Title = Path.GetFileNameWithoutExtension(item.Name),
                    Description = item.Description ?? "No description available",
                    ImageUrl = item.ThumbnailLink ?? GetImageUrl(item.Id),
                    ReleaseDate = item.CreatedTime ?? DateTime.Now,
                    ExpiresInDays = 30
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching movie with ID: {fileOrFolderId}");
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

    public string GetVideoStreamUrl(string folderId)
    {
        // Return the base URL for HLS streaming - will serve playlist from this folder
        return $"/Movie/hls/{folderId}/playlist.m3u8";
    }

    public async Task<string?> GetPlaylistFileIdAsync(string folderId)
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var request = service.Files.List();
            
            // Look for .m3u8 playlist file in the folder
            request.Q = $"'{folderId}' in parents and (name contains '.m3u8' or name = 'output.m3u8' or name = 'playlist.m3u8') and trashed=false";
            request.Fields = "files(id, name)";
            request.PageSize = 10;

            var result = await request.ExecuteAsync();

            if (result.Files != null && result.Files.Count > 0)
            {
                var playlistFile = result.Files.FirstOrDefault();
                _logger.LogInformation($"Found playlist file: {playlistFile?.Name} (ID: {playlistFile?.Id})");
                return playlistFile?.Id;
            }

            _logger.LogWarning($"No playlist file found in folder {folderId}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error finding playlist in folder {folderId}");
            return null;
        }
    }

    public async Task<List<string>> GetChunkFileIdsAsync(string folderId)
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var request = service.Files.List();
            
            // Look for .ts chunk files in the folder
            request.Q = $"'{folderId}' in parents and name contains '.ts' and trashed=false";
            request.Fields = "files(id, name)";
            request.PageSize = 1000; // Support many chunks

            var result = await request.ExecuteAsync();

            if (result.Files != null)
            {
                _logger.LogInformation($"Found {result.Files.Count} chunk files in folder {folderId}");
                return result.Files.Select(f => f.Id).ToList();
            }

            return new List<string>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error finding chunks in folder {folderId}");
            return new List<string>();
        }
    }

    public async Task<string?> FindFileInFolderAsync(string folderId, string fileName)
    {
        try
        {
            var service = await GetDriveServiceAsync();
            var request = service.Files.List();
            
            request.Q = $"'{folderId}' in parents and name = '{fileName}' and trashed=false";
            request.Fields = "files(id, name)";
            request.PageSize = 1;

            var result = await request.ExecuteAsync();
            var file = result.Files?.FirstOrDefault();
            
            if (file != null)
            {
                _logger.LogInformation($"Found file {fileName} in folder {folderId}: {file.Id}");
                return file.Id;
            }

            _logger.LogWarning($"File {fileName} not found in folder {folderId}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error finding file {fileName} in folder {folderId}");
            return null;
        }
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
