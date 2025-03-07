namespace PrivateSession.DTOs;

public class Movie
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public string ImageUrl { get; set; }
    
    public DateTime ReleaseDate { get; set; }
    
    public int ExpiresInDays { get; set; }
}