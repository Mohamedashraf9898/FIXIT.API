using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Repositories;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
			if (craftsMan == null)
				return NotFound();

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
		[HttpPost]
		public async Task<IActionResult> CreateCraftsMan([FromBody] CreateCraftsManDto dto)
		{
			if (dto == null)
				return BadRequest();

			await _craftsManService.CreateCraftsManAsync(dto);
			return CreatedAtAction(nameof(GetAllCraftsMen), null);
		}

		[HttpPut("{id:int}")]
		public  IActionResult UpdateCraftsMan(int id, [FromBody] UpdateCraftsManDto dto)
		{
			if (dto == null || dto.Id != id)
				return BadRequest();

			bool updated =  _craftsManService.UpdateCraftsMan(id, dto);
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
