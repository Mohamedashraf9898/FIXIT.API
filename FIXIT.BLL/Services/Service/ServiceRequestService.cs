using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Helper.UploadHandler;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.BLL.Services.Service.Payment;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FIXIT.BLL.Services.Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICraftsManRepo _craftsmanRepository;
        private readonly IGenericRepository<Client> _clientRepository;
        private readonly IPaymentService paymentService;
        private readonly IOfferRepository _offerRepository;
        private readonly IAvailabilityService _availabilityService;
        private readonly ITimeOffService _timeOffService;
        private readonly INotificationService _notificationService;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _transactionRepo;
        private readonly IMapper _mapper;
        private readonly UploadHandler _uploadHandler;
        private readonly ITimeSlotRepository _timeSlotRepo;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            ICraftsManRepo craftsmanRepository,
            IWalletRepository walletRepo,
            IWalletTransactionRepository transactionRepo,
            IMapper mapper,
            IGenericRepository<Client> clientRepository,
            IPaymentService paymentService,
            IOfferRepository offerRepository,
            IAvailabilityService availabilityService,
            UploadHandler uploadHandler,
            ITimeOffService timeOffService,
            INotificationService notificationService,
            ITimeSlotRepository timeSlotRepo)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _craftsmanRepository = craftsmanRepository;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
            _clientRepository = clientRepository;
            this.paymentService = paymentService;
            _offerRepository = offerRepository;
            _availabilityService = availabilityService;
            _uploadHandler = uploadHandler;
            _timeOffService = timeOffService;
            _notificationService = notificationService;
            _timeSlotRepo = timeSlotRepo;
        }
        public async Task<ReturnedServiceRequestDto> CreateServiceRequestAsync(CreateServiceRequestDto dto)
        {
            string? imagePath = null;
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Service Request Data Can not be null");
            if (dto.ServiceRequestImage != null)
            {
                imagePath = _uploadHandler.Upload(dto.ServiceRequestImage, "ServiceRequest");
            }
            var serviceRequest = _mapper.Map<ServicesRequest>(dto);
            if (imagePath != null)
                serviceRequest.ServiceRequestImage = imagePath;
            await EnsureServiceRequestLocationAsync(serviceRequest);
            serviceRequest.ServiceEndTime = serviceRequest.ServiceStartTime.AddMinutes(60);
            await _serviceRequestRepository.AddAsync(serviceRequest);
            if (serviceRequest.ClientId <= 0 || serviceRequest.ServiceId <= 0)
                throw new ValidationException("ClientId or ServiceId is invalid.");
                
            if (string.IsNullOrEmpty(serviceRequest.Description))
                throw new ValidationException("Description cannot be empty.");

           
            _serviceRequestRepository.Save();
            //var returnedDto = _mapper.Map<ReturnedServiceRequestDto>(serviceRequest);
            var returnedDto = new ReturnedServiceRequestDto
            {
                ServicesRequestId = serviceRequest.ServicesRequestId
            };

           /// var paymentResult = await paymentService.CreateOrUpdatePaymentIntent(serviceRequest.ServicesRequestId);
            _serviceRequestRepository.Update(serviceRequest, serviceRequest.ServicesRequestId);
            _serviceRequestRepository.Save();


            return returnedDto;


        }
       

        public async Task<bool> DeleteServiceRequest(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid Service Request ID");

            var serviceRequest = await _serviceRequestRepository.GetAsync(id);
            if (serviceRequest == null)
                throw new KeyNotFoundException($"Service Request with ID {id} not found");

            ValidateServiceRequestTime(serviceRequest);

            _serviceRequestRepository.Delete(id);
            _serviceRequestRepository.Save();

            return true;
        }

        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestAsync()
        {
            var serviceRequests = await _serviceRequestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(serviceRequests);
        }

        public async Task<ReadServiceRequestDto> GetServiceRequestByIdAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid ID");

            var serviceRequest = await _serviceRequestRepository.GetAsync(id);
            if (serviceRequest == null)
                throw new KeyNotFoundException($"Service Request With ID::{id} not found");

            return _mapper.Map<ReadServiceRequestDto>(serviceRequest);
        }

        public async Task<bool> UpdateServiceRequest(int id, UpdateServiceRequestDto dto)
        {
               if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _serviceRequestRepository.GetAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Service Request with ID {id} not found");

            ValidateServiceRequestTime(existing);

            _mapper.Map(dto, existing);
            if(dto.ServiceRequestImage != null)
            {
                // Delete old picture if exists
                if (!string.IsNullOrEmpty(existing.ServiceRequestImage))
                {
                    var oldPath = Path.Combine("wwwroot", existing.ServiceRequestImage);
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                // Upload new picture
                existing.ServiceRequestImage = _uploadHandler.Upload(dto.ServiceRequestImage);
            }
            await EnsureServiceRequestLocationAsync(existing);

            var updated = _serviceRequestRepository.Update(existing, id);
            if (updated) _serviceRequestRepository.Save();

            return updated;
        }
        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForCraftsManById(int craftsManId)
        {
            if (craftsManId <= 0)
                throw new ArgumentException("Craftsman ID must be greater than zero.", nameof(craftsManId));

            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            var existed = serviceRequests
                .Where(sr => sr.CraftsManId == craftsManId).OrderByDescending(sr => sr.RequestAt)
                .ToList();

            if (!existed.Any())
                return Enumerable.Empty<ReadServiceRequestDto>();

            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(existed);
        }

        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForClientById(int clientId)
        {
            if (clientId <= 0)
                throw new ArgumentException("Client ID must be greater than zero.", nameof(clientId));

            var serviceRequests = await _serviceRequestRepository.GetAllAsync();
           

            var existed = serviceRequests
                .Where(sr => sr.ClientId == clientId).OrderByDescending(sr => sr.RequestAt)
                .ToList();

            if (!existed.Any())
                return Enumerable.Empty<ReadServiceRequestDto>();

            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(existed);
        }

        #region Helper Methods

        private void ValidateServiceRequestTime(ServicesRequest serviceRequest)
        {
            var nowUtc = DateTime.UtcNow;
            var requestedUtc = serviceRequest.ServiceStartTime.ToUniversalTime();

            var remainingTime = requestedUtc - nowUtc;

            if (remainingTime.TotalHours <= 1)
                throw new InvalidOperationException("Cannot modify the service request less than one hour before or after the scheduled time.");
        }

        private async Task EnsureServiceRequestLocationAsync(ServicesRequest serviceRequest)
        {
            if (string.IsNullOrEmpty(serviceRequest.Location))
            {
                var client = await _clientRepository.GetAsync(serviceRequest.ClientId);
                if (client != null)
                    serviceRequest.Location = client.Location;
            }
        }
        #endregion
        #region ForPaymentService
        //osama added a payment method
        public async Task<bool> CompleteServiceRequestAsync(int serviceRequestId)
 {
     var serviceRequest = await _serviceRequestRepository.GetAsync(serviceRequestId);
     if (serviceRequest == null)
         throw new KeyNotFoundException("Service request not found.");

     if (serviceRequest.Status == ServiceRequestStatus.Completed)
         throw new InvalidOperationException("This service request is already completed.");

     if (serviceRequest.TotalAmount <= 0)
         throw new InvalidOperationException("Invalid service amount.");

     serviceRequest.Status = ServiceRequestStatus.Completed;
         decimal commissionRate = 0.25m;
     if (serviceRequest.TotalAmount <= 500)
     {
         commissionRate = 0.25m;
     }
     else if (serviceRequest.TotalAmount> 500 && serviceRequest.TotalAmount <= 2000)
     {
         commissionRate = 0.20m;
     }
     else if (serviceRequest.TotalAmount > 2000)
     {
         commissionRate = 0.15m;
     }
     decimal? netAmount = serviceRequest.TotalAmount * (1 - commissionRate);

     var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(serviceRequest.CraftsManId ??0);
     if (wallet == null)
         throw new Exception("Wallet not found for this craftsman.");

     wallet.Balance += netAmount ?? 0;

     var transactionDto = new CreateWalletTransactionDto
     {
         WalletId = wallet.Id,
         ServiceRequestId = serviceRequest.ServicesRequestId,
         Amount = netAmount,
         CreatedAt = DateTime.Now,
         Transactionmethod=Transactionmethod.Deposits
         
     };

     var transaction = _mapper.Map<WalletTransaction>(transactionDto);
     await _transactionRepo.AddAsync(transaction);


     _walletRepo.Save();
     _transactionRepo.Save();
     _serviceRequestRepository.Save();

     return true;
        }


        #endregion




        //public async Task<bool> UpdateServiceRequestStartAtTime(int id, ConfirmStartatTimeDto dto)
        //{
        //    if (dto == null)
        //        throw new ArgumentNullException(nameof(dto));

        //    var existing = await _serviceRequestRepository.GetAsync(id);
        //    if (existing == null)
        //        throw new KeyNotFoundException($"Service Request with ID {id} not found");

        //    var targetSlot = await _timeSlotRepo.GetSlotByDateAndTimeAsync(
        //             dto.CraftsManId,
        //             dto.ServiceStartTime
        //                                  );
        //    if (targetSlot == null)
        //        throw new ValidationException("This time slot does not exist in the craftsman's schedule.");

        //    if (targetSlot.Status != SlotStatus.Available)
        //        throw new ValidationException("Sorry, this time slot is already booked.");

        //    targetSlot.Status = SlotStatus.Booked;
        //    _timeSlotRepo.Update(targetSlot, targetSlot.Id);
        //    _timeSlotRepo.Save();
        //    _mapper.Map(dto, existing);



        //    var updated = _serviceRequestRepository.Update(existing, id);
        //    if (updated) _serviceRequestRepository.Save();

        //    var notificationDto = new CreateNotificationDto
        //    {
        //        ServiceRequestId = existing.ServicesRequestId,
        //        CraftsManId = existing.CraftsManId,
        //        ClientId = existing.ClientId,
        //        Title = "Service Request Scheduled",

        //        Message = $"Client has confirmed the time slot: {existing.ServiceStartTime}",
        //       SenderType = NotificationSenderType.Client ,
        //       Type=NotificationType.SelectCraftsman,
        //        IsRead = false 

        //    };
        //    await _notificationService.CreateFromClientAsync(notificationDto);

        //    return updated;
        //}
        public async Task<bool> UpdateServiceRequestStartAtTime(int id, ConfirmStartatTimeDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var existing = await _serviceRequestRepository.GetAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Service Request with ID {id} not found");

            var targetSlot = await _timeSlotRepo.GetSlotByDateAndTimeAsync(
                    existing.CraftsManId ?? dto.CraftsManId,
                    dto.ServiceStartTime
            );

            if (targetSlot == null)
                throw new ValidationException("This time slot does not exist in the craftsman's schedule.");

            if (targetSlot.Status != SlotStatus.Available)
                throw new ValidationException("Sorry, this time slot is already booked.");
            var oldslot = await _timeSlotRepo.GetSlotByRequestIdAsync(id);
            if (oldslot != null)
            {
                oldslot.Status = SlotStatus.Available;
                _timeSlotRepo.Update(oldslot, oldslot.Id);
            }
            targetSlot.Status = SlotStatus.Booked;
            targetSlot.ServiceRequestId = existing.ServicesRequestId;


            _timeSlotRepo.Update(targetSlot, targetSlot.Id);

            _timeSlotRepo.Save();

            _mapper.Map(dto, existing);

            int duration = existing.EstimatedDurationMinutes ?? 60;
            existing.ServiceEndTime = existing.ServiceStartTime.AddMinutes(duration);

            var updated = _serviceRequestRepository.Update(existing, id);

            if (updated)
            {
                _serviceRequestRepository.Save();

                var notificationDto = new CreateNotificationDto
                {
                    ServiceRequestId = existing.ServicesRequestId,
                    CraftsManId = existing.CraftsManId,
                    ClientId = existing.ClientId,
                    Title = "Service Request Scheduled",
                    Message = $"Client has confirmed the time slot: {existing.ServiceStartTime}",
                    SenderType = NotificationSenderType.Client,
                    Type = NotificationType.SelectCraftsman,
                    IsRead = false
                };
                await _notificationService.CreateFromClientAsync(notificationDto);
            }

            return updated;
        }
        public async Task<bool> CancelServiceRequestAsync(int serviceRequestId, CancelServiceRequestDto dto)
        {
            var serviceRequest = await _serviceRequestRepository.GetAsync(serviceRequestId);
            if (serviceRequest == null)
                throw new KeyNotFoundException("Service request not found.");

            // Only allow cancellation for InProgress requests
            if (serviceRequest.Status != ServiceRequestStatus.InProgress)
                throw new InvalidOperationException("Only in-progress service requests can be cancelled.");

            // Update status to Cancelled
            serviceRequest.Status = ServiceRequestStatus.Cancelled;
            serviceRequest.IsCancelled = true;

            _serviceRequestRepository.Update(serviceRequest, serviceRequestId);
            _serviceRequestRepository.Save();

            // Get service name
            var serviceName = serviceRequest.Service?.ServiceName ?? "Service";

            // Determine notification type
            var notificationType = dto.ReasonType == "craftsman_no_show"
                ? NotificationType.CraftsmanNoShow
                : NotificationType.ServiceCancelled;

            // Build notification message for craftsman
            var messageForCraftsman = dto.ReasonType == "craftsman_no_show"
                ? $"{dto.ClientName} has cancelled service request #{serviceRequestId} ({serviceName}) because you did not show up. Reason: {dto.Reason}"
                : $"{dto.ClientName} has cancelled service request #{serviceRequestId} ({serviceName}). Reason: {dto.Reason}";

            // Build notification message for admin
            var messageForAdmin = $"Client {dto.ClientName} ({dto.ClientEmail}) has cancelled service request #{serviceRequestId} ({serviceName}). " +
                $"Reason Type: {(dto.ReasonType == "craftsman_no_show" ? "Craftsman No-Show" : "Client Request")}. " +
                $"Reason: {dto.Reason}. Please process refund.";

            // Send notification to Craftsman
            if (serviceRequest.CraftsManId.HasValue)
            {
                var craftsmanNotification = new CreateNotificationDto
                {
                    ServiceRequestId = serviceRequestId,
                    CraftsManId = serviceRequest.CraftsManId,
                    ClientId = serviceRequest.ClientId,
                    Title = "Service Request Cancelled",
                    Message = messageForCraftsman,
                    SenderType = NotificationSenderType.Client,
                    Type = notificationType,
                    IsRead = false
                };
                await _notificationService.CreateFromClientAsync(craftsmanNotification);
            }

            // Send notification to Admin
            var adminNotification = new CreateNotificationDto
            {
                ServiceRequestId = serviceRequestId,
                CraftsManId = serviceRequest.CraftsManId,
                ClientId = serviceRequest.ClientId,
                Title = "Refund Required - Service Cancelled",
                Message = messageForAdmin,
                SenderType = NotificationSenderType.Client,
                Type = notificationType,
                IsRead = false
            };
            await _notificationService.CreateForAdminAsync(adminNotification);

            return true;
        }

        public async Task<IEnumerable<ReadServiceRequestDto>> GetRequestsByStatusAsync(ServiceRequestStatus status)
        {
            var allRequests = await _serviceRequestRepository.GetAllAsync();

            var filtered = allRequests.Where(r => r.Status == status);

            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(filtered);

        }
        public async Task<IEnumerable<ReadServiceRequestDto>> GetRequestsByClientAndStatusAsync(int clientId, ServiceRequestStatus status)
        {
            var allRequests = await _serviceRequestRepository.GetAllAsync();

            var filtered = allRequests
                            .Where(r => r.ClientId == clientId && r.Status == status);

            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(filtered);
        }

        public async Task<IEnumerable<ReadServiceRequestDto>> GetRequestsByCraftsmanAndStatusAsync(int craftsManId, ServiceRequestStatus status)
        {
          var allRequests = await _serviceRequestRepository.GetAllAsync();
            var filtered = allRequests
                            .Where(r => r.CraftsManId == craftsManId && r.Status == status);
            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(filtered);
        }
    }
}
  

