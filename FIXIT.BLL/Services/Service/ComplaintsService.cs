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
            Client client = null;
            CraftsMan craftsman = null;
            if (dto.ClientId.HasValue)
                client = await _dbContext.Clients.FindAsync(dto.ClientId.Value);
            if (dto.CraftsManId.HasValue)
                craftsman = await _dbContext.CraftsMan.FindAsync(dto.CraftsManId.Value);

            if (serviceRequest == null)
                throw new ArgumentException("ServiceRequest not found.");
            if (dto.ClientId.HasValue && client == null)
                throw new ArgumentException("Client not found.");
            if (dto.CraftsManId.HasValue && craftsman == null)
                throw new ArgumentException("Craftsman not found.");
            if (!dto.ClientId.HasValue && !dto.CraftsManId.HasValue)
                throw new ArgumentException("Either ClientId or CraftsManId must be provided.");

            var complaint = new Complaint
            {
                ServiceRequestId = dto.ServiceRequestId,
                ClientId = dto.ClientId,
                CraftsManId = dto.CraftsManId,
                Content = dto.Content,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _complaintsRepo.AddComplaintAsync(complaint);

            // Send email to support
            var subject = "New Complaint Submitted";
            var body = $@"
                <b>Client Name:</b> {(client != null ? client.FName + " " + client.LName : "-")}<br/>
                <b>Craftsman Name:</b> {(craftsman != null ? craftsman.FName + " " + craftsman.LName : "-")}<br/>
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
                CraftsManId = complaint.CraftsManId,
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
                CreatedAt = c.CreatedAt,
                AdminResponse = c.AdminResponse,
                RespondedAt = c.RespondedAt
            }).ToList();
        }

        public async Task<ResponseComplaintDto> RespondToComplaintAsync(RespondToComplaintDto dto)
        {
            var complaint = await _complaintsRepo.GetByIdAsync(dto.ComplaintId);
            if (complaint == null)
                throw new ArgumentException("Complaint not found.");

            complaint.AdminResponse = dto.AdminResponse;
            complaint.RespondedAt = DateTime.UtcNow;
            complaint.Status = dto.Status;
            await _complaintsRepo.UpdateComplaintAsync(complaint);

            return new ResponseComplaintDto
            {
                Id = complaint.Id,
                ServiceRequestId = complaint.ServiceRequestId,
                ClientId = complaint.ClientId,
                Content = complaint.Content,
                Status = complaint.Status,
                CreatedAt = complaint.CreatedAt,
                AdminResponse = complaint.AdminResponse,
                RespondedAt = complaint.RespondedAt
            };
        }
    }
}
