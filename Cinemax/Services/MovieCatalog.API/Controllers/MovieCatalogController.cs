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
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
    {
        var movies = await _repository.GetMovies();
        return Ok(movies);
    }

    [HttpGet("{id}", Name = "GetMovie")]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMovieById(string id)
    {
        var movie = await _repository.GetMovieById(id);
        if (movie == null) return NotFound();
        return Ok(movie);
    }

    [Route("Genre/{genre}")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByGenre(string genre)
    {
        var movies = await _repository.GetMoviesByGenre(genre);
        return Ok(movies);
    }

    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<Movie>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<ActionResult<Movie>> CreateMovie([FromBody] Movie movie)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _repository.CreateMovie(movie);
        return CreatedAtRoute("GetMovie", new { id = movie.Id }, movie);
    }
    
    [HttpPut]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMovie([FromBody] Movie movie)
    {
        var result = await _repository.UpdateMovie(movie);
        if(!result)
            return NotFound(null);
        return Ok();
    }

    [HttpDelete]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType( StatusCodes.Status404NotFound)]

    public async Task<ActionResult> DeleteMovie(string id)
    {
        var result = await _repository.DeleteMovie(id);
        if(!result)
            return NotFound(null);
        return Ok();
    }
}