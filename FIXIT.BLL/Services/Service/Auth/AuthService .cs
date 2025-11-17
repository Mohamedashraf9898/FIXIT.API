using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.ClientDTOs;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.Identity;
using FIXIT.BLL.Exceptions;
using FIXIT.BLL.Services.Intrfaces;
using FIXIT.BLL.Services.IService.IAuth;
using FIXIT.DAL.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FIXIT.BLL.Services.Service.Auth
{
    public class AuthService : IAuthService
    {
        
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly SignInManager<ApplicationUser> _signInManager;
            private readonly IClientService _clientService;
            private readonly ICraftsManService _craftsManService;
            private readonly IConfiguration _configuration;

            public AuthService(
                UserManager<ApplicationUser> userManager,
                SignInManager<ApplicationUser> signInManager,
                IClientService clientService,
                ICraftsManService craftsManService,
                IConfiguration configuration)
            {
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
                Token = GenerateJwtToken(user, role)
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
                throw new ValidationException();

            await _userManager.AddToRoleAsync(user, "Client");

           
            await _clientService.CreateClientAsync(new CreateClientDTO
            {
                FName = dto.FName,
                LName = dto.LName,
                Location = dto.Location,
                PhoneNumber = dto.PhoneNumber,
                ProfileImage = dto.ProfileImage,
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
                Token = GenerateJwtToken(user, "Client")
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
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ValidationException(errors);
            }
            await _userManager.AddToRoleAsync(user, "CraftsMan");

          
            await _craftsManService.CreateCraftsManAsync(new CreateCraftsManDto
            {
                FName = dto.FName,
                LName = dto.LName,
                Location = dto.Location,
                PhoneNumber = dto.PhoneNumber,
                ProfileImage = dto.ProfileImage,
                Describtion = dto.Description,
                HourlyRate = dto.HourlyRate,
                ExperienceOfYears = dto.ExperienceOfYears,
                Gender = dto.Gender,
                NationalId = dto.NationalId,
                DateOfBirth = dto.DateOfBirth,
                NormalizedEmail = user.NormalizedEmail!
            });

            return new UserDto
            {
                Id = user.Id,
                FName = user.FName,
                LName = user.LName,
                Email = user.Email,
                Role = "CraftsMan",
                Token = GenerateJwtToken(user, "CraftsMan")
            };
        }

        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

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
    }

}
