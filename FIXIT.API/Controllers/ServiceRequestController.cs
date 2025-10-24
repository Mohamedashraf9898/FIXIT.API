using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FIXIT.API.Controllers
{
    public class ServiceRequestController : Controller
    {
        //injection 
        private readonly
        public IActionResult Index()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<ActionResult> GetMyServiceRequest()
        //{
        //    var client = client.await _dbContext.Clients.FindAsync(Id);
        //    if (client == null)
        //        return NotFound("Client not found.");
        //    var requests = await _dbContext.ServiceRequest
        //                    .

                        //.Include(r => r.CraftsMan)
                        //.Include(r => r.Service)
                        //.Where(r => r.Id ==Id)
                        //.OrderByDescending(r => r.RequestedAt)
                        //.Select(r => new
                        //{
                        //    r.RequestId,
                        //    r.Description,
                        //    r.Status,
                        //    r.RequestedAt,
                        //    r.CostEstimate,
                        //    CraftsMan = r.CraftsMan.FName,
                        //})
                        //.ToListAsync();

    //    return Ok(requests); 
    //}
}
}
