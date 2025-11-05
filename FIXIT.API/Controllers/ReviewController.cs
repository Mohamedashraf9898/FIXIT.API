using FIXIT.BLL.DTOs.ReviewDTOs;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _reviewService.GetAllReviewsAsync();
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDTO reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdReview = await _reviewService.CreateReviewAsync(reviewDto);
                return Ok(createdReview);
            }
            catch (Exception ex)
            {
                
                return BadRequest(ex.Message);
            }
        }

        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDTO reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedReview = await _reviewService.UpdateReviewAsync(id, reviewDto);
                return Ok(updatedReview);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewService.DeleteReviewAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Returns a 404 if not found
            }
        }

        // --- ADD THIS "GET BY CRAFTSMAN" ENDPOINT ---

        // GET /api/Reviews/craftsman/12 (where 12 is the craftsman's id)
        [HttpGet("craftsman/{craftsmanId}")]
        public async Task<IActionResult> GetReviewsForCraftsman(int craftsmanId)
        {
            // This will return the list of reviews, or an empty list []
            var reviews = await _reviewService.GetReviewsForCraftsmanAsync(craftsmanId);
            return Ok(reviews);
        }



    }
}
