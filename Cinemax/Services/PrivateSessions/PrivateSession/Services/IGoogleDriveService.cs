namespace PrivateSession.Services;

public interface IGoogleDriveService
{
    string GetDirectDownloadUrl(string fileId);
    string GetImageUrl(string movieId);
    string GetVideoStreamUrl(string movieId, string filename = null);
    Task<Stream> GetFileStreamAsync(string fileId);
}
