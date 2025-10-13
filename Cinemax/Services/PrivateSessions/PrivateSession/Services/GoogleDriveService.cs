namespace PrivateSession.Services;

/// <summary>
/// Service for handling Google Drive file operations and URL generation
/// Base Google Drive folder: https://drive.google.com/drive/folders/1Jx8Uj4GGLrNKSxp3HLchraQFiZrXuc_d?usp=drive_link
/// </summary>
public class GoogleDriveService : IGoogleDriveService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GoogleDriveService> _logger;
    private readonly string _baseUrl;

    public GoogleDriveService(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleDriveService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _baseUrl = _configuration.GetValue<string>("GoogleDrive:BaseUrl");
    }

    public string GetDirectDownloadUrl(string fileId)
    {
        // Convert Google Drive file ID to direct download URL
        return $"https://drive.google.com/uc?export=download&id={fileId}";
    }

    public string GetImageUrl(string movieId)
    {
        // For now, we'll assume the image file ID follows a pattern
        // In a real implementation, you'd have a mapping of movieId to actual Google Drive file IDs
        // Example: movieId "1" -> Google Drive file ID "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74mHxYnpkI"
        return GetDirectDownloadUrl($"{movieId}/image");
    }

    public string GetVideoStreamUrl(string movieId, string filename = null)
    {
        // For HLS streaming, we need to construct the URL based on the movie ID and filename
        // In a real implementation, you'd map these to actual Google Drive file IDs
        if (!string.IsNullOrEmpty(filename))
        {
            return GetDirectDownloadUrl($"{movieId}/{filename}");
        }
        
        // Default to the main m3u8 file for HLS streaming
        return GetDirectDownloadUrl($"{movieId}/output.m3u8");
    }

    public async Task<Stream> GetFileStreamAsync(string fileId)
    {
        try
        {
            var downloadUrl = GetDirectDownloadUrl(fileId);
            _logger.LogInformation($"Fetching file stream from Google Drive: {downloadUrl}");
            
            var response = await _httpClient.GetAsync(downloadUrl);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadAsStreamAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching file stream for fileId: {fileId}");
            throw;
        }
    }
}
