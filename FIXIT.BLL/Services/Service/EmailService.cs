using FIXIT.BLL.DTOs;
using FIXIT.BLL.Services.IService;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendContactEmailAsync(ContactFormDto contactData)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_mailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(_mailSettings.AdminEmail));
            email.Subject = $"Contact Us Form Submission from {contactData.FullName}";

            var body = $@"
                <b>Full Name:</b> {contactData.FullName}<br/>
                <b>Email:</b> {contactData.Email}<br/>
                <b>Phone:</b> {contactData.Phone ?? "N/A"}<br/>
                <b>Message:</b><br/>{contactData.Message}
            ";

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.SenderPassword);
                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while sending the email.", ex);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}