using System.Threading.Tasks;

namespace FIXIT.BLL.Services.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
