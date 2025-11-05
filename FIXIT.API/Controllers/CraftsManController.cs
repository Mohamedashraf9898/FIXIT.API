using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Repositories;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CraftsManController : ControllerBase
	{	
		private readonly ICraftsManService craftsManService;
		public CraftsManController(ICraftsManService craftsManService,ICraftsManRepo craftsManRepo )
		{
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

			craftsManService.DeleteCraftsMan(id);

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
          
           
            if (craftsManDto.Id==id )
			{
				if(craftsManService.UpdateCraftsMan(id, craftsManDto))
                    return NoContent();
            }
			
			
            return NotFound();
        }

		[HttpGet("byname")]

		public IActionResult GetByName(string fName, string lName)
		{
			// Implementation for getting a CraftsMan by name
			var craftsMan = craftsManService.GetCraftsByNameAsync(fName,lName);
			if (craftsMan is null) return NoContent();
			return Ok(craftsMan);
		}
	}
}
