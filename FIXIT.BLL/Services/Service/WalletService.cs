using AutoMapper;
using FIXIT.BLL.DTOs.WalletDTos;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;

namespace FIXIT.BLL.Services.Service
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepo;
        private readonly IWalletTransactionRepository _transactionRepo;
        private readonly IMapper _mapper;

        public WalletService(IWalletRepository walletRepo, IWalletTransactionRepository transactionRepo, IMapper mapper)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }

     
        public async Task<WalletDto> GetWalletAsync(int craftsManId)
        {
            var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(craftsManId);
            if (wallet == null)
                return null;

            return _mapper.Map<WalletDto>(wallet);
        }

 
        public async Task<bool> AddFundsAsync(CreateWalletTransactionDto dto)
        {
            var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(dto.CraftsManId);
            if (wallet == null) return false;

           
            decimal commissionRate = 0.25m;
            decimal netAmount = dto.Amount * (1 - commissionRate);

            
            wallet.Balance += netAmount;

        
            var transaction = _mapper.Map<WalletTransaction>(dto);
            transaction.WalletId = wallet.Id;
            transaction.Amount = netAmount;
            transaction.CreatedAt = DateTime.Now;
          
            await _transactionRepo.AddAsync(transaction);
            _walletRepo.Save();

            return true;
        }

        public async Task<bool> WithdrawFundsAsync(CreateWalletTransactionDto dto)
        {
            var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(dto.CraftsManId);
            if (wallet == null || wallet.Balance < dto.Amount)
                return false;

            wallet.Balance -= dto.Amount;

            var transaction = _mapper.Map<WalletTransaction>(dto);
            transaction.WalletId = wallet.Id;
            transaction.Amount = dto.Amount;
            transaction.CreatedAt = DateTime.Now;

            await _transactionRepo.AddAsync(transaction);
            _walletRepo.Save();

            return true;
        }

        public async Task<IEnumerable<WalletTransactionDto>> GetWalletTransactionsAsync(int craftsManId)
        {
            var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(craftsManId);
            if (wallet == null)
                return Enumerable.Empty<WalletTransactionDto>();

            var transactions = await _transactionRepo.GetAllByWalletIdAsync(wallet.Id);
            return _mapper.Map<IEnumerable<WalletTransactionDto>>(transactions);
        }
    }

}