using AutoMapper;
using FIXIT.BLL.DTOs.NotificationDtos;
using FIXIT.BLL.DTOs.OfferDto;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class OfferService : IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ITimeSlotRepository _timeSlotRepo;

        public OfferService(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            IMapper mapper,
             INotificationService notificationService,
             ITimeSlotRepository timeSlotRepo)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _timeSlotRepo = timeSlotRepo;
        }


        public async Task<ReturnedOfferDto> SelectCraftsmanAsync(ClientSelectCraftsmanDto dto)
        {
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found.");

            request.CraftsManId = dto.CraftsmanId;
            request.Status = ServiceRequestStatus.WaitingForCraftsmanResponse;

            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();

            var offerDto = new ClientSelectCraftsmanDto
            {
                ServiceRequestId = dto.ServiceRequestId,
                CraftsmanId = dto.CraftsmanId
            };

            var offer = _mapper.Map<Offer>(offerDto);

            offer.SuggestedPrice = request.SuggestedPrice;

            await _offerRepository.AddAsync(offer);
            _offerRepository.Save();
            var returnedDto = new ReturnedOfferDto
            {
                Id = offer.Id
            };


            return returnedDto;
        }


        //public async Task<bool> ClientRespondToOfferAsync(ClientRespondDto dto)
        //{
        //    var offer = await _offerRepository.GetAsync(dto.OfferId);
        //    if (offer == null)
        //        throw new KeyNotFoundException("Offer not found");

        //    var request = await _serviceRequestRepository.GetAsync(offer.ServiceRequestId);
        //    if (request == null)
        //        throw new KeyNotFoundException("Service request not found");
        //    switch (dto.Decision)
        //    {
        //        case ClientDecision.Accept:
        //            offer.Status = OfferStatus.AcceptedByClient;
        //            request.TotalAmount = offer.Amount;
        //            request.Status = ServiceRequestStatus.WaitingForClientPayment;
        //            break;

        //        case ClientDecision.Reject:
        //            offer.Status = OfferStatus.RejectedByClient;
        //            request.Status = ServiceRequestStatus.RejectedByClient;
        //            break;
        //    }


        //    _offerRepository.Update(offer, offer.Id);
        //    _serviceRequestRepository.Update(request, request.ServicesRequestId);


        //    _offerRepository.Save();
        //    _serviceRequestRepository.Save();

        //    return true;
        //}
        public async Task<ReturnedOfferDto> ClientRespondToOfferAsync(ClientRespondDto dto)
        {
            var offer = await _offerRepository.GetAsync(dto.OfferId);
            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            var request = await _serviceRequestRepository.GetAsync(offer.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            NotificationType notificationType;
            string notificationTitle;
            string notificationMessage;
            int OfferId;

            switch (dto.Decision)
            {
                case ClientDecision.Accept:
                    offer.Status = OfferStatus.AcceptedByClient;
                    request.TotalAmount = offer.Amount;
                    request.Status = ServiceRequestStatus.WaitingForClientPayment;

                    notificationType = NotificationType.ClientAcceptedOffer;
                    notificationTitle = "Client Accepted Your Offer";
                    notificationMessage = $"Client has accepted your offer of {offer.Amount} EGP.";
                    break;

                case ClientDecision.Reject:
                    offer.Status = OfferStatus.RejectedByClient;
                    request.Status = ServiceRequestStatus.RejectedByClient;

                    notificationType = NotificationType.ClientRejectedOffer;
                    notificationTitle = "Client Rejected Your Offer";
                    notificationMessage = $"Client has rejected your offer of {offer.Amount} EGP.";
                    break;

                default:
                    throw new InvalidOperationException("Invalid client decision.");
            }
            //Abdallah: Release the slot if client rejected the offer
            if (dto.Decision == ClientDecision.Reject)
            {
                await ReleaseSlotByRequestId(request.ServicesRequestId);
            }
            _offerRepository.Update(offer, offer.Id);
            _serviceRequestRepository.Update(request, request.ServicesRequestId);

            _offerRepository.Save();
            _serviceRequestRepository.Save();

            //  Create Notification (FROM CLIENT → TO CRAFTSMAN)
            var notificationDto = new CreateNotificationDto
            {
                ServiceRequestId = request.ServicesRequestId,
                CraftsManId = request.CraftsManId,
                ClientId = request.ClientId,
                Title = notificationTitle,
                Message = notificationMessage,
                OfferId=offer.Id,
                SenderType = NotificationSenderType.Client,
                Type = notificationType,
                IsRead = false // الحرفي لسه ما شافهاش
            };

            await _notificationService.CreateFromClientAsync(notificationDto);
            var returnedDto = new ReturnedOfferDto
            {
                Id = offer.Id
            };


            return returnedDto;
        }

        public async Task<ReturnedOfferDto> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto)
        {
            var offer = (await _offerRepository.GetAllAsync())
                        .FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);

            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            //if (!request.SuggestedPrice.HasValue)
            //{
            //    throw new InvalidOperationException(
            //        "Client did not specify a suggested price. Craftsman cannot accept. A new offer must be created."
            //    );
            //}

            offer.Status = OfferStatus.AcceptedByCraftsman;
            offer.UpdatedAt = DateTime.UtcNow;

            request.TotalAmount = request.SuggestedPrice;
            request.Status = ServiceRequestStatus.WaitingForClientPayment;
            request.WaitingForClientPaymentAt = DateTime.UtcNow;
            _serviceRequestRepository.Update(request, request.ServicesRequestId);

            _offerRepository.Update(offer, offer.Id);
            _offerRepository.Save();
            if (request != null) _serviceRequestRepository.Save();
            // 🔥🔥 Create Notification (FROM CRAFTSMAN → TO CLIENT)
            var notificationDto = new CreateNotificationDto
            {
                ServiceRequestId = request.ServicesRequestId,
                CraftsManId = request.CraftsManId,
                ClientId = request.ClientId,
                Title = "Craftsman Accepted Your Offer",
                Message = $"Craftsman has accepted your suggested price ({request.SuggestedPrice} EGP). Please proceed with the payment.",
                SenderType = NotificationSenderType.Craftsman,
                Type = NotificationType.CraftsmanAccepted, // استخدم النوع اللي يناسبك
                OfferId=offer.Id,
                 IsRead = false // مهم: العميل لسه ما شافهاش

            };

            await _notificationService.CreateFromCraftsmanAsync(notificationDto);
            var returnedDto = new ReturnedOfferDto
            {
                Id = offer.Id
            };


            return returnedDto;
        }

        public async Task<ReturnedOfferDto> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto)
        {
            var offer = (await _offerRepository.GetAllAsync())
                        .FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);

            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            offer.Status = OfferStatus.RejectedByCraftsman;
            offer.UpdatedAt = DateTime.UtcNow;
            _offerRepository.Update(offer, offer.Id);

            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request != null) _serviceRequestRepository.Save();
            _offerRepository.Save();

            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            // عدل حالة الـ service request
            request.Status = ServiceRequestStatus.RejectedByCraftsman;
            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();


            // 🔥 Create Notification (FROM CRAFTSMAN → TO CLIENT)
            var notificationDto = new CreateNotificationDto
            {
                ServiceRequestId = request.ServicesRequestId,
                CraftsManId = request.CraftsManId,
                ClientId = request.ClientId,
                Title = "Craftsman Rejected Your Offer",
                Message = $"Craftsman has rejected your service request.",
                SenderType = NotificationSenderType.Craftsman,
                Type = NotificationType.CraftsmanRejected, // تأكد من إضافته في Enum
                OfferId=offer.Id,
                 IsRead= false // مهم: العميل لسه ما شافهاش
            };
            //Abdallah: Release the slot as the craftsman rejected the request
            await ReleaseSlotByRequestId(dto.ServiceRequestId);
            await _notificationService.CreateFromCraftsmanAsync(notificationDto);
            var returnedDto = new ReturnedOfferDto
            {
                Id = offer.Id
            };


            return returnedDto;
        }


        public async Task<ReturnedOfferDto> CraftsmanNewOfferAsync(CraftsManNewOfferDto dto)
        {
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            var currentOffer = (await _offerRepository.GetAllAsync())
                               .FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);

            if (currentOffer == null)
                throw new KeyNotFoundException("Offer not found.");

            currentOffer.Amount = dto.FinalAmount;
            currentOffer.Status = OfferStatus.NewOfferFromCraftsman;
            currentOffer.UpdatedAt = DateTime.UtcNow;

            _offerRepository.Update(currentOffer, currentOffer.Id);
            //if (!request.SuggestedPrice.HasValue)
            //    throw new InvalidOperationException("SuggestedPrice is null!");

            request.Status = ServiceRequestStatus.WaitingForClientDecision;
            _serviceRequestRepository.Update(request, request.ServicesRequestId);

            _offerRepository.Save();
            _serviceRequestRepository.Save();
         
            var notificationDto = new CreateNotificationDto
            {
                ServiceRequestId = request.ServicesRequestId,
                CraftsManId = request.CraftsManId,
                ClientId = request.ClientId,
                Title = "Craftsman Submitted a New Offer",
                Message = $"Craftsman has submitted a new offer Please review and decide.",
                FinalAmount=dto.FinalAmount,    
                SenderType = NotificationSenderType.Craftsman,
                Type = NotificationType.NewOfferFromCraftsman, 
                OfferId = currentOffer.Id,
                Description = dto.Description,
                IsRead = false 
            };
            await _notificationService.CreateFromCraftsmanAsync(notificationDto);
            var returnedDto = new ReturnedOfferDto
            {
                Id = currentOffer.Id
            };



            return returnedDto;

        }


        public async Task<bool> UpdateTotalAmountAsync(int serviceRequestId, decimal finalAmount)
        {
            var request = await _serviceRequestRepository.GetAsync(serviceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            request.TotalAmount = finalAmount;
            var updated = _serviceRequestRepository.Update(request, request.ServicesRequestId);
            if (updated) _serviceRequestRepository.Save();

            return updated;
        }

        public async Task<ReadOfferId> GetOfferById(int id)
        {
            var offer = await _offerRepository.GetAsync(id);
            if (offer == null)
                throw new KeyNotFoundException("Offer Not found");
            var readOffer =  _mapper.Map<ReadOfferId>(offer);
            return readOffer;
        }



        public async Task ReleaseSlotByRequestId(int requestId)
        {
            var slot = await _timeSlotRepo.GetSlotByRequestIdAsync(requestId);

            if (slot != null)
            {
                slot.Status = SlotStatus.Available; 
                slot.ServiceRequestId = null;      

                _timeSlotRepo.Update(slot, slot.Id);
                _timeSlotRepo.Save();
            }
        }

        public async Task<bool> CraftsmanApologizeAsync(CraftsmanApologizeDto dto)
        {
            // 1. Get the service request
            var serviceRequest = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (serviceRequest == null)
                throw new KeyNotFoundException($"Service request with ID {dto.ServiceRequestId} not found.");
            // 2. Validate status is InProgress
            if (serviceRequest.Status != ServiceRequestStatus.InProgress)
                throw new InvalidOperationException("Can only apologize for service requests that are In Progress.");
            // 3. Update status to CancelledByCraftsman
            serviceRequest.Status = ServiceRequestStatus.CancelledByCraftsman;
            // 4. Free up the time slot if exists
            var timeSlot = await _timeSlotRepo.GetSlotByRequestIdAsync(serviceRequest.ServicesRequestId);
            if (timeSlot != null)
            {
                timeSlot.Status = SlotStatus.Available;
                timeSlot.ServiceRequestId = null;
                _timeSlotRepo.Update(timeSlot, timeSlot.Id);
                _timeSlotRepo.Save();
            }
            // 5. Save service request changes
            _serviceRequestRepository.Update(serviceRequest, serviceRequest.ServicesRequestId);
            _serviceRequestRepository.Save();
            // 6. Notify the Client (using existing method)
            var clientNotification = new CreateNotificationDto
            {
                ServiceRequestId = serviceRequest.ServicesRequestId,
                CraftsManId = serviceRequest.CraftsManId,
                ClientId = serviceRequest.ClientId,
                Title = "Craftsman Cancelled Service",
                Message = string.IsNullOrEmpty(dto.Reason)
                    ? "The craftsman has apologized and cancelled. Please choose: Get Refund or Select New Craftsman."
                    : $"The craftsman apologized: \"{dto.Reason}\". Please choose: Get Refund or Select New Craftsman.",
                SenderType = NotificationSenderType.Craftsman,
                Type = NotificationType.CraftsmanApologized,
                IsRead = false
            };
            await _notificationService.CreateFromCraftsmanAsync(clientNotification);
            // 7. Notify Admin (using existing method)
            var adminNotification = new CreateNotificationDto
            {
                ServiceRequestId = serviceRequest.ServicesRequestId,
                CraftsManId = null,
                ClientId = serviceRequest.ClientId,
                Title = "Craftsman Apology Alert",
                Message = $"Craftsman (ID: {serviceRequest.CraftsManId}) apologized for Service Request #{serviceRequest.ServicesRequestId}. Client needs to decide: Refund or New Craftsman.",
                SenderType = NotificationSenderType.Craftsman,
                Type = NotificationType.CraftsmanApologized,
                IsRead = false
            };
            await _notificationService.CreateForAdminAsync(adminNotification);
            return true;
        }   
    }
}
