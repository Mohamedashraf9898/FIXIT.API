using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.Service;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService ics;

        public ClientController(IClientService ICS)
        {
            ics = ICS;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var clients=await ics.GetAllClientsAsync();
            return Ok(clients);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
			var client = await ics.GetClientsByIdAsync(id);
			return Ok(client);
		}
        [HttpPost]
        public async Task<IActionResult> Add(CreateClientDTO createClientDTO)
        {
            await ics.CreateClientAsync(createClientDTO);
            return Created();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateClientDTO clientdto)
        {
            if (clientdto.Id == id)
            {
                if (await ics.UpdateClientAsync(id, clientdto))
                    return NoContent();
            }
            return NotFound();
        }

		[HttpDelete("{id:int}")]
		public IActionResult Delete(int id)
        {
            ics.DeleteClient(id);
            return NoContent();
        }
        [HttpGet("GetByEmail")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var Client = await ics.GetClientByEmail(email);
            return Ok(Client);
        }
    }
}
