using System.Threading.Tasks;
using FIXIT.BLL.DTOs;

namespace FIXIT.BLL.Services.IService
{
    public interface IEmailService
    {
        Task SendContactEmailAsync(ContactFormDto contactData);
    }
}