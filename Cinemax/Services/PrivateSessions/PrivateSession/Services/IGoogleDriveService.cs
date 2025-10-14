using PrivateSession.DTOs;

namespace PrivateSession.Services;

public interface IGoogleDriveService
{
    Task<IEnumerable<Movie>> GetMoviesFromDriveAsync();
    Task<Movie?> GetMovieByIdAsync(string folderId);
    string GetDirectDownloadUrl(string fileId);
    string GetImageUrl(string fileId);
    string GetVideoStreamUrl(string folderId);
    Task<Stream> GetFileStreamAsync(string fileId);
    Task<string?> GetPlaylistFileIdAsync(string folderId);
    Task<List<string>> GetChunkFileIdsAsync(string folderId);
    Task<string?> FindFileInFolderAsync(string folderId, string fileName);
}
