using FIXIT.BLL.DTOs;
using FIXIT.BLL.DTOs.Identity;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IEmailService
    {
        Task SendContactEmailAsync(ContactFormDto contactData);
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);


    }
}