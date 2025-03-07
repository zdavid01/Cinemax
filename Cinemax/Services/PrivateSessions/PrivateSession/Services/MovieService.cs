using PrivateSession.DTOs;

namespace PrivateSession.Services;

public class MovieService : IMovieService
{
    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        var movies = new List<Movie>();
        movies.Add(new Movie()
        {
            Id = "1",
            Title = "Ocean Eleven",
            Description = "Danny Ocean, a gangster, rounds up a gang of associates to stage a sophisticated and elaborate casino heist which involves robbing three Las Vegas casinos simultaneously during a popular boxing event.",
        });
        return movies;
    }
    public async Task<Movie> GetMovieById(string id)
    {
        return new Movie()
        {
            Id = "1",
            Title = "Ocean Eleven",
            Description = "Ocean Eleven",
        };
    }
}