using AutoMapper;
using FIXIT.BLL.DTOs.OfferDto;
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

        public OfferService(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            IMapper mapper)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _mapper = mapper;
        }

        public async Task<bool> SelectCraftsmanAsync(ClientSelectCraftsmanDto dto)
        {
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found.");

            request.CraftsManId = dto.CraftsmanId;
            request.Status = ServiceRequestStatus.WaitingForCraftsmanResponse;

            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();
            return true;
        }

        public async Task<bool> ClientRespondToOfferAsync(ClientRespondDto dto)
        {
            var offer = await _offerRepository.GetAsync(dto.OfferId);
            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            var request = await _serviceRequestRepository.GetAsync(offer.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");
            switch (dto.Decision)
            {
                case ClientDecision.Accept:
                    offer.Status = OfferStatus.AcceptedByClient;
                    request.TotalAmount = offer.Amount;
                    request.Status = ServiceRequestStatus.WaitingForClientPayment;
                    break;

                case ClientDecision.Reject:
                    offer.Status = OfferStatus.RejectedByClient;
                    request.Status = ServiceRequestStatus.RejectedByClient;
                    break;
            }


            _offerRepository.Update(offer, offer.Id);
            _offerRepository.Save();
            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();

            return true;
        }

        public async Task<bool> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto)
        {
            var offers = await _offerRepository.GetAllAsync();
            var offer = offers.FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);
            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            offer.Status = OfferStatus.AcceptedByCraftsman;


            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request != null)
            {
                request.TotalAmount = offer.Amount;
                request.Status = ServiceRequestStatus.WaitingForClientPayment;
                _serviceRequestRepository.Update(request, request.ServicesRequestId);
            }

            _offerRepository.Update(offer, offer.Id);
            _offerRepository.Save();

            return true;
        }

        public async Task<bool> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto)
        {
            var offers = await _offerRepository.GetAllAsync();
            var offer = offers.FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);
            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            offer.Status = OfferStatus.RejectedByCraftsman;
            offer.UpdatedAt = DateTime.UtcNow;
            _offerRepository.Update(offer, offer.Id);
            _offerRepository.Save();

            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            // عدل حالة الـ service request
            request.Status = ServiceRequestStatus.RejectedByCraftsman;
            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();
            return true;
        }

        public async Task<bool> CraftsmanNewOfferAsync(CraftsManNewOfferDto dto)
        {
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            var offer = _mapper.Map<Offer>(dto);
            offer.ServiceRequestId = dto.ServiceRequestId;
            offer.Status = OfferStatus.Pending;

            await _offerRepository.AddAsync(offer);
            _offerRepository.Save();

            request.Status = ServiceRequestStatus.WaitingForClientDecision;
            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();

            return true;
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
    }
}
