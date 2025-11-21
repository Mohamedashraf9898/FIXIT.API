using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Services.IService.IAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
                return Ok(user);
          
        }

        [HttpPost("register/client")]
        public async Task<IActionResult> RegisterClient([FromBody] ClientRegisterDto dto)
        {
            
                var user = await _authService.RegisterClientAsync(dto);
                return Ok(user);
       
               
        }

        [HttpPost("register/craftsman")]
        public async Task<IActionResult> RegisterCraftsMan([FromBody] CraftsManRegisterDto dto)
        {
                 var user = await _authService.RegisterCraftsManAsync(dto);
                return Ok(user);
           
            
        }

        [Authorize]
        [HttpPost("logout")]
        public  IActionResult Logout()
        {
            // Logout logic can be handled in front-end by removing JWT
            return Ok(new { Message = "Logged out successfully" });
        }

    }
}
