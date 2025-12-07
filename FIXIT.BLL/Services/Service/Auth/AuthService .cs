using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.IAuth;
using FIXIT.DAL.DbContexts.FixitIdentityDbContext;
using FIXIT.DAL.Models.Identity;
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
          
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnAuthoraizedException("Invalid Login");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
             if (result.IsNotAllowed) throw new UnAuthoraizedException("Account not Confirmed yet");
            if (result.IsLockedOut) throw new UnAuthoraizedException("Account is Locked");
            if (!result.Succeeded) throw new UnAuthoraizedException("Invalid Login");


            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.Count > 0 ? roles[0] : string.Empty;

           
            return new UserDto
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Role = role,
                Token = await GenerateJwtTokenAsync(user, role)
            };
        }
        public async Task<UserDto> RegisterClientAsync(ClientRegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FName = dto.FName,
                LName = dto.LName,
                PhoneNumber = dto.PhoneNumber
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
                DateOfBirth=dto.DateOfBirth,
                NormalizedEmail = dto.Email
                
            });
            
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
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FName = dto.FName,
                LName = dto.LName,
                PhoneNumber = dto.PhoneNumber
             
                
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ValidationException($"Registration failed: {errors}");
            }
            await _userManager.AddToRoleAsync(user, "CraftsMan");

          
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
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return; // Do not reveal if user exists

            // Generate secure token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiry = DateTime.UtcNow.AddMinutes(15);

            // Store token in DB
            var resetToken = new PasswordResetToken
            {
                Email = dto.Email,
                Token = token,
                ExpiryDate = expiry,
                IsUsed = false
            };
            _identityDbContext.PasswordResetTokens.Add(resetToken);
            await _identityDbContext.SaveChangesAsync();

            // Build reset link
            var resetLink = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(dto.Email)}&token={Uri.EscapeDataString(token)}";

            // Send email
            var subject = "Password Reset Request";
            var body = $@"
                        Click the link to reset your password:<br>
                        <a href=""{resetLink}"">{resetLink}</a>
                        <br><br>
                        This link expires in 15 minutes.";
                    

            await _emailService.SendEmailAsync(dto.Email, subject, body);
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var token = await _identityDbContext.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Email == dto.Email && t.Token == dto.Token && !t.IsUsed);

            if (token == null || token.ExpiryDate < DateTime.UtcNow)
                return false;

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return false;

            var resetResult = await _userManager.ResetPasswordAsync(user,dto.Token, dto.NewPassword);
            if (!resetResult.Succeeded)
                return false;

            token.IsUsed = true;
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
