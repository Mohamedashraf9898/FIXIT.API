using FIXIT.BLL.DTOs;
using FIXIT.BLL.Interfaces;
using FIXIT.BLL.Repositories;
using FIXIT.BLL.Services;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CraftsManController : ControllerBase
	{
		private readonly BLL.Interfaces.IGenericRepository<CraftsMan> genericRepository;
		private readonly ICraftsManService craftsManService;

		public CraftsManController(IGenericRepository<CraftsMan> genericRepository, ICraftsManService craftsManService)
		{
			this.genericRepository = genericRepository;
			this.craftsManService = craftsManService;
		}
		[HttpGet]

		public async Task<IActionResult> GetAllCraftsMen()
		{
			List<CraftsManDto> craftsManDtos = (List<CraftsManDto>)await craftsManService.GetAllCraftsMenAsync();
			return Ok(craftsManDtos);
		}
		[HttpGet]
		[Route("{id}")]
		public async Task<IActionResult> GetCraftsManById(int id)
		{
			// Implementation for getting a CraftsMan by ID
			CraftsManDto craftsManDto = await craftsManService.GetCraftsManByIdAsync(id);
			if (craftsManDto is null)
			{
				return NotFound();
			}
			return Ok(craftsManDto);
		}
		[HttpDelete]
		[Route("{id}")]
		public IActionResult DeleteCraftsMan(int id)
		{
			// Implementation for deleting a CraftsMan by ID
			craftsManService.DeleteCraftsManAsync(id);

			return NoContent();
		}
		[HttpPost]
		public async Task<IActionResult> CreateCraftsMan([FromBody] CreateCraftsManDto craftsMan)
		{
			if (craftsMan is null)
			{
				return BadRequest();
			}
			craftsManService.CreateCraftsManAsync(craftsMan);
			return Ok();
		}
		[HttpPut]
		public async Task<IActionResult> UpdateCraftsMan(int id, UpdateCraftsManDto craftsManDto)
		{
			if (craftsManDto is null || id <= 0)
			{
				return BadRequest();
			}
			craftsManService.UpdateCraftsManAsync(id, craftsManDto);
			return NoContent();
		}
	}
}
