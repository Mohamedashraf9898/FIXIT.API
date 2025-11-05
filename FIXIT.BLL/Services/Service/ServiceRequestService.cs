using AutoMapper;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IGenericRepository<ServicesRequest> _genericRepository;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _transactionRepo;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IGenericRepository<ServicesRequest> serviceRequestRepo,
            IWalletRepository walletRepo,
            IWalletTransactionRepository transactionRepo,
            IMapper mapper)
        {
            _genericRepository = serviceRequestRepo;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }
        public async Task<bool> CreateServiceRequestAsync(CreateServiceRequestDto ServiceRequestDto)
        {
            if(ServiceRequestDto == null)
                throw new ArgumentNullException(nameof(ServiceRequestDto),"Service Request Data Can not be null");
           var serviceRequest = _mapper.Map<ServicesRequest>(ServiceRequestDto);
            await _genericRepository.AddAsync(serviceRequest);
            _genericRepository.Save();
            return true;
        }

        public async Task<bool> DeleteServiceRequest(int id)
        {
            
                var serviceRequest = await _genericRepository.GetAsync(id);
                if (serviceRequest == null)
                {
                    return false;
                }
                _genericRepository.Delete(id);
                _genericRepository.Save();
                return true;
        }

        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestAsync()
        {
            var serviceRequests = await _genericRepository.GetAllAsync();
            var result = _mapper.Map<IEnumerable<ReadServiceRequestDto>>(serviceRequests);
            return result;
        }
        // Validate for .client and .service
        public async Task<ReadServiceRequestDto> GetServiceRequestByIdAsync(int id)
        {
            if(id <= 0)
                throw new ArgumentException("Invalid ID");
            var serviceRequest = await _genericRepository.GetAsync(id);
            if (serviceRequest is null)
                throw new KeyNotFoundException($"Service Request With ID::{id} not found");
            return _mapper.Map<ReadServiceRequestDto>(serviceRequest);
        }

        public async Task<bool> UpdateServiceRequest(int id, UpdateServiceRequestDto ServiceRequestDto)
        {
            var existing = await _genericRepository.GetAsync(id);
            if(existing == null)
            {
                return false;
            }
            var updatedServiceRequest = _mapper.Map< ServicesRequest>(ServiceRequestDto);
            var result = _genericRepository.Update(updatedServiceRequest, id);
            if(result)
            {
                _genericRepository.Save();
                return true;
            }
            return false;
        }
        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestForCraftsMan(string craftsManName)
        {
            if (string.IsNullOrWhiteSpace(craftsManName))
                throw new ArgumentException("Craftsman name cannot be empty.", nameof(craftsManName));

            var serviceRequests = await _genericRepository.GetAllAsync();

            var existed = serviceRequests
                .Where(cm =>(cm.CraftsMan.FName+ " " +cm.CraftsMan.LName)
                .Contains(craftsManName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!existed.Any())
                throw new KeyNotFoundException($"No service requests found for craftsman name: {craftsManName}");
            var result = _mapper.Map<IEnumerable<ReadServiceRequestDto>>(existed);

            return result;
        }

        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestForClient(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("Client name cannot be empty.", nameof(clientName));

            // Get all service requests from repository (including relations)
            var serviceRequests = await _genericRepository.GetAllAsync();

            
            var existed = serviceRequests
                .Where(cl => (cl.Client.FName + " " + cl.Client.LName)
                .Contains(clientName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // If no matches found, return empty list
            if (!existed.Any())
                return Enumerable.Empty<ReadServiceRequestDto>();

            // Map to DTO
            var result = _mapper.Map<IEnumerable<ReadServiceRequestDto>>(existed);
            return result;
        }

        //osama added a payment method
        public async Task<bool> CompleteServiceRequestAsync(int serviceRequestId)
        {
            var serviceRequest = await _genericRepository.GetAsync(serviceRequestId);
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
            _genericRepository.Save();

            return true;
        }
    }
}
