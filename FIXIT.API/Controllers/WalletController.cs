using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Services.IService;
using FIXIT.DAL.Models;
using Microsoft.AspNetCore.Mvc;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpGet("{craftsManId}")]
        public async Task<IActionResult> GetWallet(int craftsManId)
        {
            var wallet = await _walletService.GetWalletAsync(craftsManId);
            return Ok(wallet);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddFunds([FromBody] CreateWalletTransactionDto dto)
        {
            await _walletService.AddFundsAsync(dto);
            return Ok(new
            {
                message = "Funds added successfully.",
                craftsManId = dto.CraftsManId,
                amountAdded = dto.Amount,
                date = DateTime.Now
            });
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFunds([FromBody] CreateWalletTransactionDto dto)
        {
            await _walletService.WithdrawFundsAsync(dto);
            return Ok(new
            {
                message = "Withdrawal successful.",
                craftsManId = dto.CraftsManId,
                amountWithdrawn = dto.Amount,
                date = DateTime.Now,
                withdrawmethod=dto.Transactiontype,
                withdrawmethodinfo=dto.TransationInfo,
                TransactionMethod=Transactionmethod.Withdraw
             });
        }

        [HttpGet("{craftsManId}/transactions")]
        public async Task<IActionResult> GetTransactions(int craftsManId)
        {
            var transactions = await _walletService.GetWalletTransactionsAsync(craftsManId);
            return Ok(transactions);
        }
    }
}
