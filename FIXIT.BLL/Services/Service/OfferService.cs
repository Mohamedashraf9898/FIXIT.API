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

            // تحديث ServiceRequest
            request.CraftsManId = dto.CraftsmanId;
            request.Status = ServiceRequestStatus.InProgress;
            _serviceRequestRepository.Update(request, request.ServicesRequestId);
            _serviceRequestRepository.Save();

            // إنشاء Offer تلقائي باستخدام AutoMapper
            var offerDto = new ClientSelectCraftsmanDto
            {
                ServiceRequestId = dto.ServiceRequestId,
                CraftsmanId = dto.CraftsmanId
            };

            var offer = _mapper.Map<Offer>(offerDto);

            // نسخ SuggestedPrice من ServiceRequest
            offer.SuggestedPrice = request.SuggestedPrice;

            await _offerRepository.AddAsync(offer);
            _offerRepository.Save();

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
                    request.TotalAmount = offer.Amount; // تحديث حسب الـ Offer الحالي
                    request.Status = ServiceRequestStatus.InProgress;
                    break;

                case ClientDecision.Reject:
                    offer.Status = OfferStatus.RejectedByClient;
                    request.Status = ServiceRequestStatus.Pending;
                    break;
            }

            _offerRepository.Update(offer, offer.Id);
            _serviceRequestRepository.Update(request, request.ServicesRequestId);

            // Save مرة واحدة لكل شيء
            _offerRepository.Save();
            _serviceRequestRepository.Save();

            return true;
        }


        //public async Task<bool> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto)
        //{
        //    var offers = await _offerRepository.GetAllAsync();
        //    var offer = offers.FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);
        //    if (offer == null)
        //        throw new KeyNotFoundException("Offer not found");

        //    offer.Status = OfferStatus.AcceptedByCraftsman;
        //    _offerRepository.Update(offer, offer.Id);
        //    _offerRepository.Save();

        //    return true;
        //}
        public async Task<bool> CraftsmanAcceptRequestAsync(CraftsmanAcceptDto dto)
        {
            var offer = (await _offerRepository.GetAllAsync())
                        .FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);

            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            offer.Status = OfferStatus.AcceptedByCraftsman;

            // تحديث ServiceRequest لو متاح
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request != null)
            {
                request.TotalAmount = offer.Amount; // لو Craftsman وافق على السعر النهائي
                request.Status = ServiceRequestStatus.InProgress;
                _serviceRequestRepository.Update(request, request.ServicesRequestId);
            }

            _offerRepository.Update(offer, offer.Id);

            _offerRepository.Save();
            if (request != null) _serviceRequestRepository.Save();

            return true;
        }

        //public async Task<bool> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto)
        //{
        //    var offers = await _offerRepository.GetAllAsync();
        //    var offer = offers.FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);
        //    if (offer == null)
        //        throw new KeyNotFoundException("Offer not found");

        //    offer.Status = OfferStatus.RejectedByCraftsman;
        //    _offerRepository.Update(offer, offer.Id);
        //    _offerRepository.Save();

        //    return true;
        //}
        public async Task<bool> CraftsmanRejectRequestAsync(CraftsmanRejectDto dto)
        {
            var offer = (await _offerRepository.GetAllAsync())
                        .FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);

            if (offer == null)
                throw new KeyNotFoundException("Offer not found");

            offer.Status = OfferStatus.RejectedByCraftsman;

            // تحديث حالة الطلب
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request != null)
            {
                request.Status = ServiceRequestStatus.Pending;
                _serviceRequestRepository.Update(request, request.ServicesRequestId);
            }

            _offerRepository.Update(offer, offer.Id);

            _offerRepository.Save();
            if (request != null) _serviceRequestRepository.Save();

            return true;
        }


        public async Task<bool> CraftsmanNewOfferAsync(CraftsManNewOfferDto dto)
        {
            var request = await _serviceRequestRepository.GetAsync(dto.ServiceRequestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found");

            // جلب الـ Offer الحالي
            var currentOffer = (await _offerRepository.GetAllAsync())
                               .FirstOrDefault(o => o.ServiceRequestId == dto.ServiceRequestId);

            if (currentOffer == null)
                throw new KeyNotFoundException("Offer not found. There must be an existing offer.");

            // تأكد من SuggestedPrice
            if (!request.SuggestedPrice.HasValue)
                throw new InvalidOperationException("SuggestedPrice is null!");

            // لو Craftsman وافق على الـ SuggestedPrice
            if (dto.NewAmount == request.SuggestedPrice.Value)
            {
                currentOffer.Amount = request.SuggestedPrice.Value;
                currentOffer.Status = OfferStatus.AcceptedByCraftsman;

                request.TotalAmount = request.SuggestedPrice.Value;
                request.Status = ServiceRequestStatus.InProgress;
            }
            else // لو Craftsman عمل تعديل على السعر مختلف عن SuggestedPrice
            {
                currentOffer.Amount = dto.NewAmount;
                currentOffer.Status = OfferStatus.NewOfferFromCraftsman;

                request.Status = ServiceRequestStatus.Pending;
            }

            // تحديث كل شيء مرة واحدة
            _offerRepository.Update(currentOffer, currentOffer.Id);
            _serviceRequestRepository.Update(request, request.ServicesRequestId);

            _offerRepository.Save();
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
