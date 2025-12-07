using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger, IConfiguration configuration)
        {   
            _settings = options.Value;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string from, string recipients, string subject, string body)
        {
            // Resolve settings (prefer explicit args, fallback to configuration/options)
            var fromAddress = !string.IsNullOrWhiteSpace(from)
                ? from
                : _configuration["EmailSettings:FromEmail"] ?? _settings.From;

            if (string.IsNullOrWhiteSpace(recipients))
            {
                _logger.LogError("Email send failed: Recipients address is empty.");
                throw new ArgumentException("Recipient address cannot be empty.", nameof(recipients));
            }

            var emailMessage = new MailMessage();
            emailMessage.From = new MailAddress(fromAddress, _configuration["EmailSettings:FromName"] ?? _settings.FromName);
            emailMessage.To.Add(recipients);
            emailMessage.Subject = subject ?? string.Empty;
            emailMessage.Body = body ?? string.Empty;
            emailMessage.IsBodyHtml = true;

            var host = _configuration["EmailSettings:Host"] ?? _settings.SmtpServer;
            var portStr = _configuration["EmailSettings:Port"];
            var port = _settings.Port;
            if (!string.IsNullOrWhiteSpace(portStr) && int.TryParse(portStr, out var parsed)) port = parsed;

            var smtpUser = _configuration["EmailSettings:Username"] ?? _settings.Username;
            var smtpPass = _configuration["EmailSettings:Password"] ?? _settings.Password;
            var enableSslStr = _configuration["EmailSettings:EnableSsl"];
            var enableSsl = _settings.EnableSSL;
            if (!string.IsNullOrWhiteSpace(enableSslStr) && bool.TryParse(enableSslStr, out var parsedSsl)) enableSsl = parsedSsl;

            if (string.IsNullOrWhiteSpace(host))
            {
                _logger.LogError("Email send failed: SMTP host is not configured.");
                throw new ArgumentException("SMTP host is not configured.", "EmailSettings:Host");
            }

            using var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(smtpUser ?? string.Empty, smtpPass ?? string.Empty),
                EnableSsl = enableSsl
            };

            try
            {
                await smtpClient.SendMailAsync(emailMessage);
                _logger.LogInformation("Email sent to {Recipients}", recipients);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "Error sending email to {Recipients}", recipients);
                throw;
            }
        }
    }
}
