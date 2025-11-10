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
			var createdReview = await _reviewService.CreateReviewAsync(reviewDto);
			return StatusCode(StatusCodes.Status201Created, createdReview);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateReview(int id, UpdateReviewDTO reviewDto)
		{
			var updatedReview = await _reviewService.UpdateReviewAsync(id, reviewDto);
			return Ok(updatedReview);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteReview(int id)
		{
			await _reviewService.DeleteReviewAsync(id);
			return NoContent();
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetReviewById(int id)
		{
			var review = await _reviewService.GetReviewByIdAsync(id);
			return Ok(review);
		}

		[HttpGet("craftsman/{craftsmanId}")]
		public async Task<IActionResult> GetReviewsForCraftsman(int craftsmanId)
		{
			var reviews = await _reviewService.GetReviewsForCraftsmanAsync(craftsmanId);
			return Ok(reviews);
		}



	}
}
