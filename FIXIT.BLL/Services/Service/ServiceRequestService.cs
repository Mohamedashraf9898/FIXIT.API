using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.BLL.Services.Service.Payment;
using FIXIT.DAL.Models;


namespace FIXIT.BLL.Services.Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICraftsManRepo _craftsmanRepository;
        private readonly IGenericRepository<Client> _clientRepository;
        private readonly IPaymentService paymentService;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _transactionRepo;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            ICraftsManRepo craftsmanRepository,
            IWalletRepository walletRepo,
            IWalletTransactionRepository transactionRepo,
            IMapper mapper,
            IGenericRepository<Client> clientRepository,
            IPaymentService paymentService
            )
        {
            _serviceRequestRepository = serviceRequestRepository;
            _craftsmanRepository = craftsmanRepository;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
            _clientRepository = clientRepository;
            this.paymentService = paymentService;
        }
        public async Task<bool> CreateServiceRequestAsync(CreateServiceRequestDto ServiceRequestDto)
        {
            if (ServiceRequestDto == null)
                throw new ArgumentNullException(nameof(ServiceRequestDto), "Service Request Data Can not be null");
            var serviceRequest = _mapper.Map<ServicesRequest>(ServiceRequestDto);
            var isExist = await _serviceRequestRepository.GetByIntentId(serviceRequest.PaymentIntentId!);
            if (isExist is not null)
            {
                _serviceRequestRepository.Delete(isExist.ServicesRequestId);
              await paymentService.CreateOrUpdatePaymentIntent(serviceRequest.ServicesRequestId);
            }
            
            await EnsureServiceRequestLocationAsync(serviceRequest);
            await _serviceRequestRepository.AddAsync(serviceRequest);
            _serviceRequestRepository.Save();

            return true;
        }

        public async Task<bool> DeleteServiceRequest(int id)
        {

            if (id <= 0)
                throw new ArgumentException("Invalid Service Request ID");

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
            var result = _mapper.Map<IEnumerable<ReadServiceRequestDto>>(serviceRequests);
            return result;
        }
        // Validate for .client and .service
        public async Task<ReadServiceRequestDto> GetServiceRequestByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid ID");
            var serviceRequest = await _serviceRequestRepository.GetAsync(id);
            if (serviceRequest is null)
                throw new KeyNotFoundException($"Service Request With ID::{id} not found");
            return _mapper.Map<ReadServiceRequestDto>(serviceRequest);
        }

        public async Task<bool> UpdateServiceRequest(int id, UpdateServiceRequestDto ServiceRequestDto)
        {

            if (ServiceRequestDto == null)
                throw new ArgumentNullException(nameof(ServiceRequestDto), "Service Request Data cannot be null");

            var existing = await _serviceRequestRepository.GetAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Service Request with ID {id} not found");
            ValidateServiceRequestTime(existing);

            _mapper.Map(ServiceRequestDto, existing);
            await EnsureServiceRequestLocationAsync(existing);
            var result = _serviceRequestRepository.Update(existing, id);
            if (result)
            {
                _serviceRequestRepository.Save();

                return true;
            }
            return false;

        }
        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestsForCraftsManById(int craftsManId)
        {
            if (craftsManId <= 0)
                throw new ArgumentException("Craftsman ID must be greater than zero.", nameof(craftsManId));

            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            var existed = serviceRequests
                .Where(sr => sr.CraftsManId == craftsManId)
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
                .Where(sr => sr.ClientId == clientId)
                .ToList();

            if (!existed.Any())
                return Enumerable.Empty<ReadServiceRequestDto>();

            return _mapper.Map<IEnumerable<ReadServiceRequestDto>>(existed);
        }


        #region Helper Method
        private void ValidateServiceRequestTime(ServicesRequest serviceRequest)
        {
            var remainingTime = serviceRequest.ServiceAt - DateTime.Now;
            if (remainingTime.TotalHours <= 1 || serviceRequest.ServiceAt <= DateTime.Now)
                throw new InvalidOperationException("Cannot modify the service request less than one hour before or after the scheduled time.");
        }

        private async Task EnsureServiceRequestLocationAsync(ServicesRequest serviceRequest)
        {
            if (string.IsNullOrEmpty(serviceRequest.Location))
            {
                var client = await _clientRepository.GetAsync(serviceRequest.ClientId);
                if (client != null)
                {
                    serviceRequest.Location = client.Location;
                }
            }
        }

        public async Task<List<CraftsManDto>> GetCraftsmenByLocationAsync(int serviceRequestId)
        {
            // 1. نجيب الـ ServiceRequest
            var serviceRequest = await _serviceRequestRepository.GetAsync(serviceRequestId);
            if (serviceRequest == null || string.IsNullOrEmpty(serviceRequest.Location))
                return new List<CraftsManDto>();

            // 2. نفصل المحافظة، المدينة، القرية
            var locationParts = serviceRequest.Location.Split(',').Select(p => p.Trim()).ToArray();
            var governorate = locationParts.ElementAtOrDefault(0) ?? "";
            var city = locationParts.ElementAtOrDefault(1) ?? "";
            var village = locationParts.ElementAtOrDefault(2) ?? "";

            // 3. نجيب كل الـ Craftsmen
            var allCraftsmen = await _craftsmanRepository.GetAllAsync();

            // 4. فلترة على المحافظة
            var craftsmenInGovernorate = allCraftsmen
                .Where(c => !string.IsNullOrEmpty(c.Location))
                .Where(c => c.Location.Split(',').ElementAtOrDefault(0).Trim()
                              .Equals(governorate, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!craftsmenInGovernorate.Any())
                return new List<CraftsManDto>(); // مفيش حد في المحافظة → نرجع فاضي

            // 5. فلترة على المدينة داخل المحافظة
            var craftsmenInCity = craftsmenInGovernorate
                .Where(c => c.Location.Split(',').ElementAtOrDefault(1).Trim()
                              .Equals(city, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!craftsmenInCity.Any())
                return _mapper.Map<List<CraftsManDto>>(craftsmenInGovernorate); // مفيش حد في المدينة → نرجع المحافظة

            // 6. فلترة على القرية داخل المدينة والمحافظة
            var craftsmenInVillage = craftsmenInCity
                .Where(c => c.Location.Split(',').ElementAtOrDefault(2).Trim()
                              .Equals(village, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!craftsmenInVillage.Any())
                return _mapper.Map<List<CraftsManDto>>(craftsmenInCity); // مفيش حد في القرية → نرجع المدينة

            // 7. لو فيه حد في القرية → نرجعهم
            return _mapper.Map<List<CraftsManDto>>(craftsmenInVillage);
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
            decimal netAmount = serviceRequest.TotalAmount * (1 - commissionRate);

            var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(serviceRequest.CraftsManId);
            if (wallet == null)
                throw new Exception("Wallet not found for this craftsman.");

            wallet.Balance += netAmount;

            var transactionDto = new CreateWalletTransactionDto
            {
                WalletId = wallet.Id,
                ServiceRequestId = serviceRequest.ServicesRequestId,
                Amount = netAmount,

                CreatedAt = DateTime.Now
            };

            var transaction = _mapper.Map<WalletTransaction>(transactionDto);
            await _transactionRepo.AddAsync(transaction);


            _walletRepo.Save();
            _transactionRepo.Save();
            _serviceRequestRepository.Save();

            return true;
        } 
        #endregion

    }
}
  

