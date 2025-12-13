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
        private readonly IConfiguration _config;

        public AccountController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }
        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
                return Ok(user);
          
        }
        #endregion

        #region register/client
        [HttpPost("register/client")]
        public async Task<IActionResult> RegisterClient([FromBody] ClientRegisterDto dto)
        {

            var user = await _authService.RegisterClientAsync(dto);
            return Ok(user);


        }
        #endregion

        # region register/craftsman
        [HttpPost("register/craftsman")]
        public async Task<IActionResult> RegisterCraftsMan([FromBody] CraftsManRegisterDto dto)
        {
            var user = await _authService.RegisterCraftsManAsync(dto);
            return Ok(user);


        }
        #endregion
        
        # region forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var frontendUrl = _config["FrontendUrl"] ?? "https://myfrontend.com/";
            await _authService.ForgotPasswordAsync(dto, frontendUrl);
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }
        #endregion

        # region reset password
        [HttpGet("reset-password/validate")]
        public async Task<IActionResult> ValidateToken([FromQuery] ValidateTokenDto dto)
        {
            var isValid = await _authService.ValidateResetTokenAsync(dto);
            if (!isValid)
                return BadRequest(new { error = "Invalid or expired token." });
            return Ok(new { message = "Token is valid." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result)
                return BadRequest(new { error = "Invalid token or password." });
            return Ok(new { message = "Password has been reset successfully." });
        }
        #endregion

        #region verifyAccountAfterReg
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var result = await _authService.ConfirmEmailAsync(dto.Email, dto.Token);
            if (!result)
                return BadRequest(new { error = "Invalid or expired token." });

            return Ok(new { message = "Email has been successfully verified." });
        }

        [HttpPost("resend-verification-email")]
        public async Task<IActionResult> ResendVerificationEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { error = "Email is required." });

            var result = await _authService.ResendVerificationEmailAsync(email);
            // Always return OK to avoid leaking user existence, unless you specifically want to show error
            // But here we'll just say sent.
            return Ok(new { message = "If the account exists and is unverified, a new verification link has been sent." });
        }
        #endregion

        #region logout
        [Authorize]
        [HttpPost("logout")]
        public  IActionResult Logout()
        {
            // Logout logic can be handled in front-end by removing JWT
            return Ok(new { Message = "Logged out successfully" });
        }
        #endregion
    }
}
