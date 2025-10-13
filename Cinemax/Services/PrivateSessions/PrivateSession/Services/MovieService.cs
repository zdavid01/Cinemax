using PrivateSession.DTOs;

namespace PrivateSession.Services;

public class MovieService : IMovieService
{
    private readonly IGoogleDriveService _googleDriveService;

    public MovieService(IGoogleDriveService googleDriveService)
    {
        _googleDriveService = googleDriveService;
    }
    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        var movies = new List<Movie>();
        movies.Add(new Movie()
        {
            Id = "1",
            Title = "Ocean Eleven",
            Description = "Danny Ocean, a gangster, rounds up a gang of associates to stage a sophisticated and elaborate casino heist which involves robbing three Las Vegas casinos simultaneously during a popular boxing event.",
            ImageUrl = _googleDriveService.GetImageUrl("1"),
            ReleaseDate = new DateTime(2001, 12, 7),
            ExpiresInDays = 30
        });
        return movies;
    }
    public async Task<Movie> GetMovieById(string id)
    {
        return new Movie()
        {
            Id = "1",
            Title = "Ocean Eleven",
            Description = "Danny Ocean, a gangster, rounds up a gang of associates to stage a sophisticated and elaborate casino heist which involves robbing three Las Vegas casinos simultaneously during a popular boxing event.",
            ImageUrl = _googleDriveService.GetImageUrl(id),
            ReleaseDate = new DateTime(2001, 12, 7),
            ExpiresInDays = 30
        };
    }
}