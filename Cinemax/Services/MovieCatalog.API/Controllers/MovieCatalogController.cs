using Microsoft.AspNetCore.Mvc;
using MovieCatalog.API.Data;
using MovieCatalog.API.Entities;
using MovieCatalog.API.Repositories.Interfaces;

namespace MovieCatalog.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class MovieCatalogController : ControllerBase
{
    IMovieRepository _repository;

    public MovieCatalogController(IMovieRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Movie>>> GetProducts()
    {
        var products = await _repository.GetMovies();
        return Ok(products);
    }
}