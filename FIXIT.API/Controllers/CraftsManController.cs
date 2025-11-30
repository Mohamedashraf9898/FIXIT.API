using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Services.Intrfaces;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CraftsManController : ControllerBase
	{
		private readonly ICraftsManService _craftsManService;

		public CraftsManController(ICraftsManService craftsManService)
		{
			_craftsManService = craftsManService;
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

	}
}
