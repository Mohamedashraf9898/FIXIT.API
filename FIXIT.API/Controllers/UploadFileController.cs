using FIXIT.BLL.Helper.UploadHandler;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly string _webRootPath;

        public UploadFileController(IWebHostEnvironment env)
        {
            _webRootPath = env.WebRootPath;
        }

        [HttpPost("Upload")]
        public IActionResult UploadFile(IFormFile file)
        {
            return Ok(new UploadHandler(_webRootPath).Upload(file));
        }
    }
}
