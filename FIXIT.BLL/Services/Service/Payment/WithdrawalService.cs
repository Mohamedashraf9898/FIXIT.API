using FIXIT.BLL.DTOs.WithdrawalDTOs;
using FIXIT.BLL.Repositories.IRepo;
using FIXIT.BLL.Services.IService.Payment;
using FIXIT.DAL.Models;
using FIXIT.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace FIXIT.BLL.Services.Service.Payment
{
    public class WithdrawalService : IWithdrawalService
    {
        private readonly IWithdrawalRepository _withdrawalRepo;
        private readonly IGenericRepository<Wallet> _walletRepo;
        private readonly IGenericRepository<WalletTransaction> _transactionRepo;
        private readonly IVodafoneCashService _vodafoneService;
        private readonly FixItDbContext _dbContext;

        public WithdrawalService(
            IWithdrawalRepository withdrawalRepo,
            IGenericRepository<Wallet> walletRepo,
            IGenericRepository<WalletTransaction> transactionRepo,
            FixItDbContext dbContext,
            IVodafoneCashService vodafoneService)
        {
            _withdrawalRepo = withdrawalRepo;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _dbContext = dbContext;
            _vodafoneService = vodafoneService;
        }

        public async Task<ReadWithdrawalDto> CreateWithdrawalAsync(int craftsManId, WithdrawalRequestDto dto)
        {
            var wallet = await _walletRepo.GetAll()
                .FirstOrDefaultAsync(w => w.CraftsManId == craftsManId);

            if (wallet == null)
                throw new Exception("Wallet not found for this craftsman.");

            if (dto.Amount > wallet.Balance)
                throw new Exception("Insufficient balance.");

            if (dto.Amount <= 50)
                throw new Exception("Amount must be greater than 50 Pounds.");


            var withdrawal = new WithdrawalRequest
            {
                CraftsManId = craftsManId,
                Amount = dto.Amount,
                PhoneNumber = dto.PhoneNumber,
                Status = WithdrawalStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            await _withdrawalRepo.AddAsync(withdrawal);

            await _withdrawalRepo.SaveAsync();

            return new ReadWithdrawalDto
            {
                Id = withdrawal.Id,
                CraftsManId = withdrawal.CraftsManId,
                Amount = withdrawal.Amount,
                PhoneNumber = withdrawal.PhoneNumber,
                Status = withdrawal.Status.ToString(),
                RequestedAt = withdrawal.RequestedAt
            };
        }

        public async Task<List<ReadWithdrawalDto>> GetPendingWithdrawalsAsync()
        {
            var list = await _withdrawalRepo.GetAll()
                .Where(w => w.Status == WithdrawalStatus.Pending)
                .ToListAsync();

            return list.Select(w => new ReadWithdrawalDto
            {
                Id = w.Id,
                CraftsManId = w.CraftsManId,
                Amount = w.Amount,
                PhoneNumber = w.PhoneNumber,
                Status = w.Status.ToString(),
                RequestedAt = w.RequestedAt,
                ProcessedAt = w.ProcessedAt
            }).ToList();
        }

       
        public async Task<ReadWithdrawalDto> ApproveAsync(int withdrawalId)
        {
            var withdrawal = await _withdrawalRepo.GetAsync(withdrawalId);
            if (withdrawal == null)
                throw new Exception("Withdrawal request not found");

            if (withdrawal.Status != WithdrawalStatus.Pending)
                throw new Exception("Withdrawal already processed");

            var wallet = await _walletRepo.GetAll()
                .FirstOrDefaultAsync(w => w.CraftsManId == withdrawal.CraftsManId);

            if (wallet == null)
                throw new Exception("Craftsman wallet not found");

            if (withdrawal.Amount > wallet.Balance)
                throw new Exception("Insufficient wallet balance");

            // 🟢 Call Vodafone Cash API to transfer money
            bool isSuccess = await _vodafoneService.TransferAsync(withdrawal.PhoneNumber, withdrawal.Amount);

            // Update withdrawal status
            withdrawal.Status = isSuccess ? WithdrawalStatus.Success : WithdrawalStatus.Failed;
            withdrawal.ProcessedAt = DateTime.UtcNow;
            _withdrawalRepo.Update(withdrawal, withdrawal.Id);

            // Update wallet only if transfer successful
            if (isSuccess)
            {
                wallet.Balance -= withdrawal.Amount;
                _walletRepo.Update(wallet, wallet.Id);

                // Record transaction
                var transaction = new WalletTransaction
                {
                    CraftsManId = withdrawal.CraftsManId,
                    ServiceRequestId = null,
                    Amount = withdrawal.Amount,
                    TransactionDate = DateTime.UtcNow,
                    Type = TransactionType.WithdrawalPending
                };
                await _transactionRepo.AddAsync(transaction);
                await _transactionRepo.SaveAsync();
            }

            await _withdrawalRepo.SaveAsync();

            return new ReadWithdrawalDto
            {
                Id = withdrawal.Id,
                CraftsManId = withdrawal.CraftsManId,
                Amount = withdrawal.Amount,
                PhoneNumber = withdrawal.PhoneNumber,
                Status = withdrawal.Status.ToString(),
                RequestedAt = withdrawal.RequestedAt,
                ProcessedAt = withdrawal.ProcessedAt
            };
        }
    }
}
