using Castle.Core.Smtp;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.IAuth;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            IEmailService emailService,
            IConfiguration configuration,
            IAuthService authService,
            Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _emailService = emailService;
            _configuration = configuration;
            _authService = authService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.LoginAsync(dto);
            return Ok(user);

        }
        #endregion
        #region Register
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
        #endregion
        //#region Forget Password
        //[HttpPost("forgot-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);

        //    await _authService.ForgotPasswordRequestAsync(dto.Email);

        //    return Ok(new { Message = "If an account exists for this email, a password reset link has been sent." });
        //}
        //#endregion
        #region Send Link

        [HttpPost("send-reset-password")]
        public async Task<IActionResult> SendResetPasswordEmail(ForgotPasswordDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user is not null)
                {
                    //generate URL =>Action property 
                    var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);//UNIQUE TOKEN for this user
                    var resetPasswordUrl = Url.Action("ResetPassword", "Account", new { email = user.Email, token = resetPasswordToken }, "https", "localhost:7058");

                    //build url Base64 encoded => https://localhost:7058/Account/ResetPassword?email=gehad@gmail.com&tfkdnvfdn,vndnfvndndnxvx
                    //unique & use for only one time &owner if this url =>token
                   
                    // Send the raw URL to email (ok)
                    await _emailService.SendEmailAsync(
                        from: _configuration["EmailSettings:FromEmail"],
                        recipients: dto.Email,
                        subject: "Reset Your Password",
                        body: resetPasswordUrl);
                    return RedirectToAction(nameof(CheckYourInbox));

                }
                ModelState.AddModelError(string.Empty, "There is No Account with this Email !");
            }
            return Ok(new { Message = "Reset password email sent successfully." });
        }
        [HttpGet("CheckYourInbox")]

        public IActionResult CheckYourInbox()
        {
            return Ok(new
            {
                Message = "Please check your email inbox for the password reset link.",
                Gmail = "https://mail.google.com/mail/u/0/#inbox"
            });
        }
        #endregion
        #region reset password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest(new { message = "Invalid email or token." });

            // فك URL encoding قبل استدعاء ResetPasswordAsync
            var urlDecodedToken = WebUtility.UrlDecode(dto.Token);

            var result = await _userManager.ResetPasswordAsync(user, urlDecodedToken, dto.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = "Password has been reset successfully." });
        }

        #endregion
        //#region Reset Password Eng Naser
        //[HttpPost("reset-password")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //await _authService.ResetPasswordAsync(dto);
        //        //HERE I NEED TO GET EMAIL , TOKEN| دول كانوا ف URL
        //        var user = await _userManager.FindByEmailAsync(dto.Email);
        //        if (user is not null)
        //        {
        //            await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);//not url encoding
        //            return RedirectToAction(nameof(SignIn));
        //        }
        //        ModelState.AddModelError(string.Empty, "Url is not valid");

        //    }
        //    return Ok(new { Message = "Password has been reset successfully." });
        //}
        //#endregion

        #region Logout
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Logout logic can be handled in front-end by removing JWT
            return Ok(new { Message = "Logged out successfully" });
        }
        #endregion

    }
}
