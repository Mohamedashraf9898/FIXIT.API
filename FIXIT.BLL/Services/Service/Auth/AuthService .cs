using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.IAuth;
using FIXIT.DAL.DbContexts.FixitIdentityDbContext;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FIXIT.BLL.Services.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IEmailService _emailService;
        private readonly IdentityDbContext _identityDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IClientService _clientService;
        private readonly ICraftsManService _craftsManService;
        private readonly IConfiguration _configuration;



        public AuthService(
            IEmailService emailService,
            IdentityDbContext identityDbContext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IClientService clientService,
            ICraftsManService craftsManService,
            IConfiguration configuration)
        {
            _emailService = emailService;
            _identityDbContext = identityDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _clientService = clientService;
            _craftsManService = craftsManService;
            _configuration = configuration;

        }

        public async Task<UserDto> LoginAsync(LoginDto dto)
        {
            // 1️⃣ البحث عن المستخدم بالإيميل
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnAuthoraizedException("Invalid Login"); // البريد أو الباسورد خطأ


            if (!user.EmailConfirmed)
                throw new UnAuthoraizedException("Please verify your email first");
            // 2️⃣ التحقق من كلمة المرور
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            // 3️⃣ التحقق من حالة الحساب
            if (!result.Succeeded)
            {
                if (result.IsNotAllowed)
                    throw new UnAuthoraizedException("Account not confirmed yet"); // لم يتم تفعيل الإيميل
                if (result.IsLockedOut)
                    throw new UnAuthoraizedException("Account is locked"); // الحساب مقفل
                throw new UnAuthoraizedException("Invalid Login"); // خطأ عام
            }

            // 4️⃣ الحصول على الرول (Client أو CraftsMan)
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.Count > 0 ? roles[0] : string.Empty;

            // 5️⃣ إنشاء JWT Token
            var token = await GenerateJwtTokenAsync(user, role);

            // 6️⃣ إرجاع معلومات المستخدم مع التوكن
            return new UserDto
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Role = role,
                Token = token
            };
        }

        public async Task<UserDto> RegisterClientAsync(ClientRegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                {
                    throw new ValidationException($"Registration failed: Email '{dto.Email}' is already taken.");
                }
                else
                {
                    // 1. Delete Orphan Client Profile (if exists) FIRST
                    try
                    {
                        var existingClient = await _clientService.GetClientByEmail(dto.Email);
                        if (existingClient != null)
                            _clientService.DeleteClient(existingClient.Id);
                    }
                    catch (NotFoundException) { /* No profile found, ok */ }
                    catch (Exception ex)
                    {
                        // 2. CRITICAL: Catch other errors (FK violation, etc) and report them!
                        throw new ValidationException($"Failed to cleanup old client profile: {ex.Message} - {ex.InnerException?.Message}");
                    }

                    // 3. Delete Identity User
                    var deleteResult = await _userManager.DeleteAsync(existingUser);
                    if (!deleteResult.Succeeded)
                    {
                        throw new ValidationException("Failed to reset existing unverified account: " + string.Join(", ", deleteResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FName = dto.FName,
                LName = dto.LName,
                PhoneNumber = dto.PhoneNumber,
                Location = dto.Location,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException($"Registration failed: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Client");

            await _clientService.CreateClientAsync(new CreateClientDTO
            {
                FName = dto.FName,
                LName = dto.LName,
                Location = dto.Location,
                PhoneNumber = dto.PhoneNumber,
                ProfileImage = $"Images\\default.png",
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                NormalizedEmail = dto.Email
            });

            // Generate Email Confirmation Token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(user.Email);
            var backendUrl = _configuration["BackendUrl"]?.TrimEnd('/') ?? "https://localhost:7083";
            //var confirmUrl = $"{backendUrl}/verify-email.html?email={encodedEmail}&token={encodedToken}";
            var verificationUrl = $"http://localhost:4200/login?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}&action=verify";

            #region HTML Email Template
            var subject = "Verify Your Email - Fixit";
            var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<title>Verify Your Email</title>
<style>
body {{ font-family: 'Cairo', Arial, sans-serif; background-color: #f8f9fa; margin:0; padding:0; }}
.container {{ max-width:600px; margin:40px auto; background-color:#fff; border-radius:16px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.1); }}
.header {{ background-color:#1E1E1E; text-align:center; padding:30px 0; }}
.header h1 {{ color:#FFD700; margin:0; font-size:28px; font-weight:700; }}
.content {{ padding:40px 30px; text-align:center; }}
.content h2 {{ color:#1E1E1E; font-size:24px; margin-top:0; }}
.content p {{ color:#6B7280; font-size:16px; line-height:1.6; margin-bottom:30px; }}
.btn {{ background-color:#FFD700; color:#1E1E1E; padding:15px 35px; text-decoration:none; border-radius:50px; display:inline-block; font-weight:bold; font-size:16px; box-shadow:0 4px 15px rgba(255,215,0,0.4); }}
.btn:hover {{ background-color:#E5C100 !important; }}
.footer {{ background-color:#f8f8f8; padding:20px; text-align:center; border-top:1px solid #eeeeee; font-size:12px; color:#888888; }}
a.link {{ color:#FFD700; text-decoration:underline; word-break:break-all; }}
</style>
</head>
<body>
<div class='container'>
  <div class='header'><h1>Fixit</h1></div>
  <div class='content'>
    <h2>Email Verification</h2>
    <p>Hello,<br>Thank you for registering. Please click the button below to verify your email:</p>
    <a href='{confirmUrl}' class='btn'>Verify Email</a>
    <p style='margin-top:30px; font-size:14px; color:#999;'>Or copy and paste this link into your browser:<br>
      <a href='{confirmUrl}' class='link'>{confirmUrl}</a>
    </p>
  </div>
  <div class='footer'>
    This link works for 15 minutes.<br>
    If you didn't register, you can safely ignore this email.
  </div>
</div>
</body>
</html>";

#endregion
            await _emailService.SendEmailAsync(user.Email, subject, body);

            return new UserDto
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Role = "Client",
                Token = await GenerateJwtTokenAsync(user, "Client")
            };
        }
        public async Task<UserDto> RegisterCraftsManAsync(CraftsManRegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                if (existingUser.EmailConfirmed)
                {
                    throw new ValidationException($"Registration failed: Email '{dto.Email}' is already taken.");
                }
                else
                {
                    // 1. Delete Orphan CraftsMan Profile (if exists) FIRST
                    try
                    {
                        var existingCraftsman = await _craftsManService.GetCraftsManByEmailAsync(dto.Email);
                        if (existingCraftsman != null && existingCraftsman.CraftsMan != null)
                            _craftsManService.DeleteCraftsMan(existingCraftsman.CraftsMan.Id);
                    }
                    catch (NotFoundException) { /* No profile found, ok */ }

                    // 2. Delete Identity User
                    var deleteResult = await _userManager.DeleteAsync(existingUser);
                    if (!deleteResult.Succeeded)
                    {
                        throw new ValidationException("Failed to reset existing unverified account. Please contact support.");
                    }
                }
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FName = dto.FName,
                LName = dto.LName,
                PhoneNumber = dto.PhoneNumber,
                Location = dto.Location,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                NationalId = dto.NationalId,
                EmailConfirmed = false // مهم: الحساب غير مفعل حتى التحقق من الإيميل
            };

            // إنشاء المستخدم في Identity
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException($"Registration failed: {errors}");
            }

            // إضافة رول CraftsMan
            await _userManager.AddToRoleAsync(user, "CraftsMan");

            // إنشاء بيانات CraftsMan في جدول مخصص
            await _craftsManService.CreateCraftsManAsync(new CreateCraftsManDto
            {
                FName = dto.FName,
                LName = dto.LName,
                Location = dto.Location,
                PhoneNumber = dto.PhoneNumber,
                ProfileImage = $"Images\\default.png",
                Describtion = dto.Description!,
                HourlyRate = dto.HourlyRate,
                ExperienceOfYears = dto.ExperienceOfYears,
                Gender = dto.Gender,
                NationalId = dto.NationalId,
                DateOfBirth = dto.DateOfBirth,
                NormalizedEmail = dto.Email,
                ServiceId = dto.ServiceId
            });

            // توليد Token لتأكيد الإيميل
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var encodedEmail = Uri.EscapeDataString(user.Email);
            var backendUrl = _configuration["BackendUrl"]?.TrimEnd('/') ?? "https://localhost:7083";
            var verificationUrl = $"http://localhost:4200/login?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}&action=verify";

            #region HTML Email Template جاهز
            var subject = "Verify Your Email - Fixit";
            var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
<meta charset='UTF-8'>
<title>Verify Your Email</title>
<style>
body {{ font-family: 'Cairo', Arial, sans-serif; background-color: #f8f9fa; margin:0; padding:0; }}
.container {{ max-width:600px; margin:40px auto; background-color:#fff; border-radius:16px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.1); }}
.header {{ background-color:#1E1E1E; text-align:center; padding:30px 0; }}
.header h1 {{ color:#FFD700; margin:0; font-size:28px; font-weight:700; }}
.content {{ padding:40px 30px; text-align:center; }}
.content h2 {{ color:#1E1E1E; font-size:24px; margin-top:0; }}
.content p {{ color:#6B7280; font-size:16px; line-height:1.6; margin-bottom:30px; }}
.btn {{ background-color:#FFD700; color:#1E1E1E; padding:15px 35px; text-decoration:none; border-radius:50px; display:inline-block; font-weight:bold; font-size:16px; box-shadow:0 4px 15px rgba(255,215,0,0.4); }}
.btn:hover {{ background-color:#E5C100 !important; }}
.footer {{ background-color:#f8f8f8; padding:20px; text-align:center; border-top:1px solid #eeeeee; font-size:12px; color:#888888; }}
a.link {{ color:#FFD700; text-decoration:underline; word-break:break-all; }}
</style>
</head>
<body>
<div class='container'>
  <div class='header'><h1>Fixit</h1></div>
  <div class='content'>
    <h2>Email Verification</h2>
    <p>Hello,<br>Thank you for registering. Please click the button below to verify your email:</p>
    <a href='{confirmUrl}' class='btn'>Verify Email</a>
    <p style='margin-top:30px; font-size:14px; color:#999;'>Or copy and paste this link into your browser:<br>
      <a href='{confirmUrl}' class='link'>{confirmUrl}</a>
    </p>
  </div>
  <div class='footer'>
    This link works for 15 minutes.<br>
    If you didn't register, you can safely ignore this email.
  </div>
</div>
</body>
</html>";

            #endregion
            // إرسال الإيميل
            await _emailService.SendEmailAsync(user.Email, subject, body);

            // إنشاء JWT Token
            return new UserDto
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Role = "CraftsMan",
                Token = await GenerateJwtTokenAsync(user, "CraftsMan")
            };
        }
        public async Task<bool> ConfirmEmailAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var decodedToken = Uri.UnescapeDataString(token);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            return result.Succeeded;
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
          {
             new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
             new Claim("id", user.Id.ToString()),
             new Claim(ClaimTypes.Role, role)
           };

            // ✅ ADD Client/Craftsman ID to JWT
            if (role == "Client")
            {
                var client = await _clientService.GetClientByEmail(user.Email!);
                if (client != null)
                {
                    claims.Add(new Claim("clientId", client.Id.ToString()));
                }
            }
            else if (role == "CraftsMan")
            {
                var craftsman = await _craftsManService.GetCraftsManByEmailAsync(user.Email!);
                if (craftsman?.CraftsMan != null)
                {
                    claims.Add(new Claim("craftsmanId", craftsman.CraftsMan.Id.ToString()));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(5),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task ForgotPasswordAsync(ForgotPasswordDto dto, string frontendUrl)
        {
            // 1) Check if user exists
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return; // Security: don't reveal user existence

            // 2) Generate secure token (RAW Identity token)
            var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 3) Save token in DB (Optional, depending on your logic, but Identity handles it mostly)
            // If you are using a custom table:
            var expiry = DateTime.UtcNow.AddMinutes(15);
            var resetToken = new PasswordResetToken
            {
                Email = dto.Email,
                Token = rawToken, // Store the raw token!
                ExpiryDate = expiry,
                IsUsed = false
            };
            _identityDbContext.PasswordResetTokens.Add(resetToken);
            await _identityDbContext.SaveChangesAsync();

            // 4) Build link - CRITICAL: Use Uri.EscapeDataString to turn '+' into '%2B'
            // This ensures the browser sends it correctly and Angular decodes it back to '+'
            var encodedToken = Uri.EscapeDataString(rawToken);
            var encodedEmail = Uri.EscapeDataString(dto.Email);

            // Ensure no double slashes in URL
            var textUrl = frontendUrl.TrimEnd('/');
            var resetUrl = $"http://localhost:4200/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(rawToken)}";
            #region  Build HTML email body
            // 5) Build HTML email body - Premium Gold & Black Theme
            var subject = "Reset Your Password - Fixit";
            var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <title>Reset Your Password</title>
    <style>
        body {{
            font-family: 'Cairo', Arial, sans-serif;
            background-color: #f8f9fa;
            margin: 0;
            padding: 0;
        }}
        .container {{
            max-width: 600px;
            margin: 40px auto;
            background-color: #ffffff;
            border-radius: 16px;
            overflow: hidden;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }}
        .header {{
            background-color: #1E1E1E;
            text-align: center;
            padding: 30px 0;
        }}
        .header h1 {{
            color: #FFD700;
            margin: 0;
            font-size: 28px;
            font-weight: 700;
            letter-spacing: 1px;
        }}
        .content {{
            padding: 40px 30px;
            text-align: center;
        }}
        .content h2 {{
            color: #1E1E1E;
            font-size: 24px;
            margin-top: 0;
        }}
        .content p {{
            color: #6B7280;
            font-size: 16px;
            line-height: 1.6;
            margin-bottom: 30px;
        }}
        .btn {{
            background-color: #FFD700;
            color: #1E1E1E;
            padding: 15px 35px;
            text-decoration: none;
            border-radius: 50px;
            display: inline-block;
            font-weight: bold;
            font-size: 16px;
            box-shadow: 0 4px 15px rgba(255, 215, 0, 0.4);
            transition: background-color 0.3s ease;
        }}
        .btn:hover {{
            background-color: #E5C100 !important;
        }}
        .footer {{
            background-color: #f8f8f8;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #eeeeee;
            font-size: 12px;
            color: #888888;
        }}
        a.link {{
            color: #FFD700;
            text-decoration: underline;
            word-break: break-all;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <!-- Header -->
        <div class='header'>
            <h1>Fixit</h1>
        </div>

        <!-- Content -->
        <div class='content'>
            <h2>Password Reset Request</h2>
            <p>
                Hello,<br>
                We received a request to reset your password for your Fixit account. If you made this request, click the button below:
            </p>

            <a href='{resetUrl}' class='btn'>Reset Password</a>

            <p style='margin-top: 30px; font-size: 14px; color: #999999;'>
                Or copy and paste this link into your browser:<br>
                <a href='{resetUrl}' class='link'>{resetUrl}</a>
            </p>
        </div>

        <!-- Footer -->
        <div class='footer'>
            This link works for 15 minutes.<br>
            If you didn't request this, you can safely ignore this email.
        </div>
    </div>
</body>
</html>";

            #endregion
            // 6) Send email
            await _emailService.SendEmailAsync(dto.Email, subject, body);
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            // 1️⃣ البحث عن المستخدم
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return false;

            // 2️⃣ البحث عن الـ Token في DB
            var resetTokenEntry = await _identityDbContext.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Email == dto.Email && t.Token == dto.Token && !t.IsUsed);

            if (resetTokenEntry == null || resetTokenEntry.ExpiryDate < DateTime.UtcNow)
                return false;

            // 3️⃣ استخدام Token المخزن مع UserManager
            //    Token لازم يكون مولّد من GeneratePasswordResetTokenAsync
            //    لو أنتِ بالفعل خزنتِه بعد GeneratePasswordResetTokenAsync فهذا سيعمل بشكل صحيح
            var resetResult = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            if (!resetResult.Succeeded)
                return false;

            // 4️⃣ تعليم الـ Token أنه مستخدم
            resetTokenEntry.IsUsed = true;
            await _identityDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateResetTokenAsync(ValidateTokenDto dto)
        {
            var token = await _identityDbContext.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Email == dto.Email && t.Token == dto.Token && !t.IsUsed);

            if (token == null || token.ExpiryDate < DateTime.UtcNow)
                return false;

            return true;
        }

    }

}
