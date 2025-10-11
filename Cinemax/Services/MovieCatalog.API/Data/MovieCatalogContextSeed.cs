using MongoDB.Driver;
using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Data;

public class MovieCatalogContextSeed
{
    public static void SeedData(IMongoCollection<Movie> moviesCollection)
    {
        var existMovies = moviesCollection.Find(p => true).Any();
        if (!existMovies)
        {
            moviesCollection.InsertManyAsync(GetConfiguredMovies());
        }
    }

    private static IEnumerable<Movie> GetConfiguredMovies()
    {
        return new List<Movie>()
        {
            new Movie()
        	{
            	Id = "602d2149e773f2a3990b47f5",
            	Title = "Star Wars: The Force Awakens",
            	Length = 136,
            	Genre = "Action / Sci-Fi",
            	Director = "J. J. Abrams",
            	Actors = "Daisy Ridley, John Boyega, Harrison Ford",
            	Description = "As a new threat to the galaxy rises, Rey, a desert scavenger, joins forces with Finn, a defected stormtrooper, and Han Solo to fight the evil First Order.",
            	ImageUrl = "https://lumiere-a.akamaihd.net/v1/images/image_0de14dca.jpeg",
            	linkToTrailer = "https://www.youtube.com/watch?v=sGbxmsDFVnE",
            	Rating = "PG-13",
				Price = 5
        	},
        	new Movie()
        	{
            	Id = "602d2149e773f2a3990b47f6",
            	Title = "Harry Potter and the Sorcerer's Stone",
            	Length = 152,
            	Genre = "Fantasy / Adventure",
            	Director = "Chris Columbus",
            	Actors = "Daniel Radcliffe, Emma Watson, Rupert Grint",
            	Description = "An orphaned boy discovers he is a wizard and attends Hogwarts School of Witchcraft and Wizardry, where he makes friends and uncovers a dark secret.",
            	ImageUrl = "https://www.originalfilmart.com/cdn/shop/files/harry_potter_and_the_sorcerers_stone_2001_original_film_art_5000x.webp?v=1684872812",
            	linkToTrailer = "https://www.youtube.com/watch?v=VyHV0BRtdxo",
            	Rating = "PG",
				Price = 15
        	},
        	new Movie()
        	{
            	Id = "602d2149e773f2a3990b47f7",
            	Title = "The Dark Knight",
            	Length = 152,
            	Genre = "Action / Crime",
            	Director = "Christopher Nolan",
            	Actors = "Christian Bale, Heath Ledger, Aaron Eckhart",
            	Description = "Batman raises the stakes in his war on crime when a new criminal mastermind, the Joker, emerges from the shadows.",
            	ImageUrl = "https://m.media-amazon.com/images/I/5151N2hUPiL._UF894,1000_QL80_.jpg",
            	linkToTrailer = "https://www.youtube.com/watch?v=EXeTwQWrcwY",
            	Rating = "PG-13",
				Price = 9
        	},
        	new Movie()
        	{
            	Id = "602d2149e773f2a3990b47f8",
            	Title = "Inception",
            	Length = 148,
            	Genre = "Sci-Fi / Thriller",
            	Director = "Christopher Nolan",
            	Actors = "Leonardo DiCaprio, Joseph Gordon-Levitt, Ellen Page",
            	Description = "A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a CEO.",
            	ImageUrl = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_.jpg",
            	linkToTrailer = "https://www.youtube.com/watch?v=YoHD9XEInc0",
            	Rating = "PG-13",
				Price = 10
        	},
        	new Movie()
        	{
            	Id = "602d2149e773f2a3990b47f9",
            	Title = "The Matrix",
            	Length = 136,
            	Genre = "Action / Sci-Fi",
            	Director = "The Wachowskis",
            	Actors = "Keanu Reeves, Laurence Fishburne, Carrie-Anne Moss",
            	Description = "A computer hacker learns the truth about the simulated reality known as the Matrix and joins a rebellion against its controllers.",
            	ImageUrl = "https://m.media-amazon.com/images/I/71PfZFFz9yL._UF894,1000_QL80_.jpg",
            	linkToTrailer = "https://www.youtube.com/watch?v=vKQi3bBA1y8",
            	Rating = "R",
				Price = 12
        	},
        	new Movie()
        	{
            	Id = "602d2149e773f2a3990b47fa",
            	Title = "Avengers: Endgame",
            	Length = 181,
            	Genre = "Action / Superhero",
            	Director = "Anthony Russo, Joe Russo",
            	Actors = "Robert Downey Jr., Chris Evans, Scarlett Johansson",
            	Description = "After the devastating events of Infinity War, the Avengers assemble once more to reverse Thanos's actions and restore balance to the universe.",
            	ImageUrl = "https://www.komar.de/media/catalog/product/cache/5/image/9df78eab33525d08d6e5fb8d27136e95/import/api-v1.1-file-public-files-pim-assets-97-ad-84-62-6284ad972eff292d45ce1a2e-images-2e-44-f8-65-65f8442ee95865a273d91872-4-4127-avengers-endgame-movie-poster-ecirgb-xl-web.jpg",
            	linkToTrailer = "https://www.youtube.com/watch?v=TcMBFSGVi1c",
            	Rating = "PG-13",
				Price = 7
			}
        };
    }
}