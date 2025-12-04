using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CraftsManController : ControllerBase
	{
		private readonly ICraftsManService _craftsManService;
        private readonly IOpenAIService _openAIService;
        public CraftsManController(ICraftsManService craftsManService, IOpenAIService openAIService)
		{
			_craftsManService = craftsManService;
            _openAIService = openAIService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllCraftsMen()
		{
			var craftsmen = await _craftsManService.GetAllCraftsMenAsync();
			return Ok(craftsmen);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCraftsManById(int id)
		{
			var craftsMan = await _craftsManService.GetCraftsManByIdAsync(id);
			return Ok(craftsMan);
		}

		[HttpGet("search")]
		public async Task<IActionResult> GetByName([FromQuery] string? fName, [FromQuery] string? lName)
		{
			var craftsmen = await _craftsManService.GetCraftsMenByNameAsync(fName, lName);
			return Ok(craftsmen);
		}

		[HttpGet("GetByLocation")]
		public async Task<IActionResult> GetByLocation(string location, string servicename)
		{
			var craftsmen = await _craftsManService.GetCraftsMenByLocationandServiceAsync(location, servicename);
			return Ok(craftsmen);
		}
		[HttpGet("GetByEmail")]
		public async Task<IActionResult> GetByEmail(string email)
		{
			var craftsman = await _craftsManService.GetCraftsManByEmailAsync(email);
			return Ok(craftsman);
		}

        [HttpGet("details/{id:int}")]
        public async Task<IActionResult> GetCraftsManDetails(int id)
        {
            var details = await _craftsManService.GetCraftsManDetailsAsync(id);
            return Ok(details);
        }


        [HttpPost]
		public async Task<IActionResult> CreateCraftsMan([FromBody]	CreateCraftsManDto dto)
		{
			await _craftsManService.CreateCraftsManAsync(dto);
			return StatusCode(StatusCodes.Status201Created);
		}

		[HttpPut("{id:int}")]
		public  async Task<IActionResult> UpdateCraftsMan(int id, [FromForm] UpdateCraftsManDto dto)
		{
			if (dto == null || dto.Id != id)
				return BadRequest();

			bool updated = await _craftsManService.UpdateCraftsManAsync(id, dto);
			if (!updated)
				return NotFound();

			return NoContent();
		}
		[HttpPut("verfication/{id:int}")]
        public async Task<IActionResult> UpdateCraftsmanVerfication(VerficationOfCraftsmanDto dto,int id )
		{
            if (dto == null )
                return BadRequest();
            bool updated = await _craftsManService.UpdateCraftsmanVerfication(dto,id);
            if (!updated)
                return NotFound();

            return NoContent();

		}

		[HttpDelete("{id:int}")]
		public IActionResult DeleteCraftsMan(int id)
		{
			_craftsManService.DeleteCraftsMan(id);
			return NoContent();
		}



        [HttpPost("verify-national-id")]
        [Authorize] // Add if you have authentication
        public async Task<IActionResult> VerifyNationalId([FromForm] IdVerificationRequestDto request)
        {
            try
            {
                // 1. Validate inputs
                if (request.FrontImage == null || request.BackImage == null)
                    return BadRequest(new { message = "Both front and back images are required" });

                // Validate file size (5MB max)
                const long maxFileSize = 5 * 1024 * 1024;
                if (request.FrontImage.Length > maxFileSize || request.BackImage.Length > maxFileSize)
                    return BadRequest(new { message = "Image size must be less than 5MB" });

                // Validate file type
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(request.FrontImage.ContentType) ||
                    !allowedTypes.Contains(request.BackImage.ContentType))
                    return BadRequest(new { message = "Only JPG and PNG images are allowed" });

                // 2. Get craftsman by email from request or JWT token
                // Option A: From request
                var craftsman = await _craftsManService.GetCraftsManByEmailAsync(request.Email);

                // Option B: From JWT token (if you have auth)
                // var email = User.FindFirst(ClaimTypes.Email)?.Value;
                // var craftsman = await _craftsManService.GetCraftsManByEmailAsync(email);

                if (craftsman?.CraftsMan == null)
                    return NotFound(new { message = "Craftsman not found" });

                // 3. Check if already verified
                if (craftsman.CraftsMan.IsVerified)
                    return BadRequest(new { message = "Craftsman is already verified" });

                // 4. Call OpenAI verification service
                var verificationResult = await _openAIService.VerifyEgyptianNationalIdAsync(
                    request.FrontImage,
                    request.BackImage);

                // 5. If valid, update verification status
                if (verificationResult.IsValid)
                {
                    var verificationDto = new VerficationOfCraftsmanDto { IsVerified = true };
                    await _craftsManService.UpdateCraftsmanVerfication(verificationDto, craftsman.CraftsMan.Id);
                }

                // 6. Return verification result
                return Ok(verificationResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Verification service error. Please try again later." });
            }
        }

    }
}
