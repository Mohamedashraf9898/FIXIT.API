using AutoMapper;
using FIXIT.API.Erorrs.Exceptions;
using FIXIT.BLL.DTOs.WalletDTos;
using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Exceptions;
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
				throw new NotFoundException(nameof(Wallet), $"Wallet not found for craftsman with ID {craftsManId}");
			return _mapper.Map<WalletDto>(wallet);
		}

		public async Task<bool> AddFundsAsync(CreateWalletTransactionDto dto)
		{
			if (dto == null)
				throw new ValidationException("Transaction data cannot be null.");
			if (dto.Amount <= 0)
				throw new ValidationException("Amount must be greater than zero.");

			var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(dto.CraftsManId);
			if (wallet == null)
				return false;

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
			if (dto == null)
				throw new ValidationException("Transaction data cannot be null.");
			if (dto.Amount <= 0)
				throw new ValidationException("Amount must be greater than zero.");

			var wallet = await _walletRepo.GetWalletByCraftsManIdAsync(dto.CraftsManId);
			if (wallet == null)
				return false;
			if (wallet.Balance < dto.Amount)
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
				throw new NotFoundException(nameof(Wallet), $"Wallet not found for craftsman with ID {craftsManId}");

			var transactions = await _transactionRepo.GetAllByWalletIdAsync(wallet.Id);

			if (transactions == null || !transactions.Any())
				throw new NotFoundException(nameof(WalletTransaction), "No transactions found for this wallet.");

			return _mapper.Map<IEnumerable<WalletTransactionDto>>(transactions);
		}
	}

}