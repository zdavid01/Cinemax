using MongoDB.Driver;
using MovieCatalog.API.Entities;

namespace MovieCatalog.API.Data;

public interface IMovieCatalogContext
{
    IMongoCollection<Movie> Movies { get; }
}