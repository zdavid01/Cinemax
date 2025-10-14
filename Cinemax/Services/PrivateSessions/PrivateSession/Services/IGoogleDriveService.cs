using PrivateSession.DTOs;

namespace PrivateSession.Services;

public interface IGoogleDriveService
{
    Task<IEnumerable<Movie>> GetMoviesFromDriveAsync();
    Task<Movie?> GetMovieByIdAsync(string fileId);
    string GetDirectDownloadUrl(string fileId);
    string GetImageUrl(string fileId);
    string GetVideoStreamUrl(string fileId);
    Task<Stream> GetFileStreamAsync(string fileId);
}
