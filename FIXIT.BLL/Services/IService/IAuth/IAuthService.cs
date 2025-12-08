using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FIXIT.BLL.DTOs.Identity;

namespace FIXIT.BLL.Services.IService.IAuth
{

    public interface IAuthService
    {
        Task<UserDto> RegisterClientAsync(ClientRegisterDto dto);
        Task<UserDto> RegisterCraftsManAsync(CraftsManRegisterDto dto);
        Task<UserDto> LoginAsync(LoginDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto, string frontendUrl);
        Task<bool> ValidateResetTokenAsync(ValidateTokenDto dto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
        Task<bool> ConfirmEmailAsync(string email, string token);

    }
}
