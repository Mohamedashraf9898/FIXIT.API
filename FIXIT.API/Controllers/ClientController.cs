using System.Threading.Tasks;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.Services;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var client =await ics.GetClientsByIdAsync(id);
            //if(client == null) {return NotFound();}
            return Ok(client);
        }
        [HttpPost]
        public async Task<IActionResult> Add(CreateClientDTO createClientDTO)
        {
            if (createClientDTO is null)
            {
                return BadRequest();
            }
            await ics.CreateClientAsync(createClientDTO);
            return Created();
        }
        [HttpPut]
       
        public ActionResult Update(int id, UpdateClientDTO clientdto)
        {
            if (clientdto.Id==id)
            {
              if(ics.UpdateClient(id, clientdto))
               return NoContent();
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            ics.DeleteClient(id);
            return NoContent();
        }
    }
}
