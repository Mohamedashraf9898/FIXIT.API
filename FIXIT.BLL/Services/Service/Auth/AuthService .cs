using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.IAuth;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FIXIT.BLL.Services.Service.Auth
{
    public class AuthService : IAuthService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IClientService _clientService;
        private readonly ICraftsManService _craftsManService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IClientService clientService,
            ICraftsManService craftsManService,
            IConfiguration configuration,
            IEmailService emailService) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _clientService = clientService;
            _craftsManService = craftsManService;
            _configuration = configuration;
            _emailService = emailService; 
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
                NormalizedEmail = user.NormalizedEmail!
                
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
                NormalizedEmail = user.NormalizedEmail!,
                ServiceId = dto.ServiceId

            });

            return new UserDto
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Role = "CraftsMan",
                Token =await GenerateJwtTokenAsync(user, "CraftsMan")
            };
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user, string role)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim("id", user.Id.ToString()),
        new Claim(ClaimTypes.Role, role)
    };

            // ✅ ADD Client/Craftsman ID to JWT
            if (role == "Client")
            {
                var client = await _clientService.GetClientByEmail(user.Email);
                if (client != null)
                {
                    claims.Add(new Claim("clientId", client.Id.ToString()));
                }
            }
            else if (role == "CraftsMan")
            {
                var craftsman = await _craftsManService.GetCraftsManByEmailAsync(user.Email);
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

        public async Task<bool> ForgotPasswordRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return true;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var resetLink = $"{_configuration["ApiUrl"]}/auth/reset-password?email={user.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(user.Email, "Reset Password", $"Click here: {resetLink}");
            return true;
        }
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new NotFoundException("User", dto.Email);

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);

            if (!result.Succeeded)
            {
                throw new ValidationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return true;
        }


    }

}
