using AutoMapper;
using FIXIT.BLL.DTOs.CraftsmanDTOs;
using FIXIT.BLL.DTOs.ServiceRequestDTOs;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Repositories.Repo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL;
using FIXIT.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIXIT.BLL.Services.Service
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IGenericRepository<Client> _clientRepository;
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _transactionRepo;
        private readonly IMapper _mapper;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IWalletRepository walletRepo,
            IWalletTransactionRepository transactionRepo,
            IMapper mapper,
            IGenericRepository<Client> clientRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
            _clientRepository = clientRepository;
        }
        public async Task<bool> CreateServiceRequestAsync(CreateServiceRequestDto ServiceRequestDto)
        {
            if (ServiceRequestDto == null)
                throw new ArgumentNullException(nameof(ServiceRequestDto), "Service Request Data Can not be null");
            var serviceRequest = _mapper.Map<ServicesRequest>(ServiceRequestDto);
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
        public async Task<IEnumerable<ReadServiceRequestDto>> GetAllServiceRequestForCraftsMan(string craftsManName)
        {
            if (string.IsNullOrWhiteSpace(craftsManName))
                throw new ArgumentException("Craftsman name cannot be empty.", nameof(craftsManName));

            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            var existed = serviceRequests
                .Where(cm => (cm.CraftsMan.FName + " " + cm.CraftsMan.LName)
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


            var serviceRequests = await _serviceRequestRepository.GetAllAsync();

            var existed = serviceRequests
                .Where(cl => (cl.Client.FName + " " + cl.Client.LName)
                .Contains(clientName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (!existed.Any())
                return Enumerable.Empty<ReadServiceRequestDto>();

            var result = _mapper.Map<IEnumerable<ReadServiceRequestDto>>(existed);
            return result;
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
