using FIXIT.BLL.DTOs.ComplaintDtos;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class ComplaintsService : IComplaintsService
    {
        private readonly IComplaintsRepository _complaintsRepo;
        private readonly IEmailService _emailService;
        private readonly FixItDbContext _dbContext;
        private readonly MailSettings _mailSettings;

        public ComplaintsService(IComplaintsRepository complaintsRepo, IEmailService emailService, FixItDbContext dbContext, IOptions<MailSettings> mailSettings)
        {
            _complaintsRepo = complaintsRepo;
            _emailService = emailService;
            _dbContext = dbContext;
            _mailSettings = mailSettings.Value;
        }

        public async Task<ResponseComplaintDto> AddComplaintAsync(CreateComplaintDto dto)
        {
            var serviceRequest = await _dbContext.ServicesRequests.FindAsync(dto.ServiceRequestId);
            var client = await _dbContext.Clients.FindAsync(dto.ClientId);

            if (serviceRequest == null)
                throw new ArgumentException("ServiceRequest not found.");
            if (client == null)
                throw new ArgumentException("Client not found.");

            var complaint = new Complaint
            {
                ServiceRequestId = dto.ServiceRequestId,
                ClientId = dto.ClientId,
                Content = dto.Content,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _complaintsRepo.AddComplaintAsync(complaint);

            // Send email to support
            var subject = "New Complaint Submitted";
            var body = $@"
                <b>Client Name:</b> {client.FName} {client.LName}<br/>
                <b>ServiceRequestId:</b> {serviceRequest.ServicesRequestId}<br/>
                <b>Complaint Content:</b> {complaint.Content}<br/>
                <b>Created At:</b> {complaint.CreatedAt:yyyy-MM-dd HH:mm:ss}
            ";
            await _emailService.SendEmailAsync(_mailSettings.AdminEmail, subject, body);

            return new ResponseComplaintDto
            {
                Id = complaint.Id,
                ServiceRequestId = complaint.ServiceRequestId,
                ClientId = complaint.ClientId,
                Content = complaint.Content,
                Status = complaint.Status,
                CreatedAt = complaint.CreatedAt
            };
        }

        public async Task<List<ResponseComplaintDto>> GetByServiceRequestIdAsync(int serviceRequestId)
        {
            var complaints = await _complaintsRepo.GetByServiceRequestIdAsync(serviceRequestId);
            return complaints.Select(c => new ResponseComplaintDto
            {
                Id = c.Id,
                ServiceRequestId = c.ServiceRequestId,
                ClientId = c.ClientId,
                Content = c.Content,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<List<ResponseComplaintDto>> GetAllAsync()
        {
            var complaints = await _complaintsRepo.GetAllAsync();
            return complaints.Select(c => new ResponseComplaintDto
            {
                Id = c.Id,
                ServiceRequestId = c.ServiceRequestId,
                ClientId = c.ClientId,
                Content = c.Content,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            }).ToList();
        }
    }
}
