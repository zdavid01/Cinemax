using Microsoft.AspNetCore.Mvc;
using Reviews.API.Entities;
using Reviews.API.Repositories;

namespace Reviews.API.Controllers;

[ApiController]
[Route("/api/v1/[controller]/")]

public class ReviewsController : ControllerBase
{
    private IReviewRepository _repository;

    public ReviewsController(IReviewRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
    {
        var reviews = await _repository.GetReviews();
        return Ok(reviews);
    }

    [HttpGet("{id}", Name = "GetReview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<ActionResult<Review>> GetReviewById(string id)
    {
        var review = await _repository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }
        
        return Ok(review);
    }
    
    [HttpGet("[action]/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status404NotFound)]

    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByUserId(string userId)
    {
        var reviews = await _repository.GetReviewByUserId(userId);
        if (reviews is null)
        {
            return NotFound(null);
        }

        return Ok(reviews);
    }
    
    [HttpGet("[action]/{movieId}")]
    [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status404NotFound)]

    public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByMovieId(string movieId)
    {
        var reviews = await _repository.GetReviewsByMovieId(movieId);
        if (reviews is null)
        {
            return NotFound(null);
        }

        return Ok(reviews);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<Review>> CreateReview([FromBody] Review review)
    {
        await _repository.CreateReview(review);
        
        return CreatedAtRoute("GetReview", new { id = review.Id }, review);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> UpdateReview(string id, [FromBody] Review review)
    {
        return Ok(await _repository.UpdateReview(review));
    }

    [HttpDelete("{id}", Name = "DeleteReview")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteReview(string id)
    {
        return Ok(await _repository.DeleteReview(id));
    }
    
}