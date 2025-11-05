using FIXIT.BLL.DTOs.WalletTransactionDTOs;
using FIXIT.BLL.Services.IService;
using Microsoft.AspNetCore.Http;
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
            if (wallet == null)
                return NotFound($"Wallet not found for craftsman with ID {craftsManId}");

            return Ok(wallet);
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddFunds([FromBody] CreateWalletTransactionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _walletService.AddFundsAsync(dto);
            if (!result)
                return BadRequest("Could not add funds. Please check craftsman ID or amount.");

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _walletService.WithdrawFundsAsync(dto);
            if (!result)
                return BadRequest("Insufficient balance or invalid craftsman ID.");

            return Ok(new
            {
                message = "Withdrawal successful.",
                craftsManId = dto.CraftsManId,
                amountWithdrawn = dto.Amount,
                date = DateTime.Now
            });
        }

    
        [HttpGet("{craftsManId}/transactions")]
        public async Task<IActionResult> GetTransactions(int craftsManId)
        {
            var transactions = await _walletService.GetWalletTransactionsAsync(craftsManId);
            if (transactions == null || !transactions.Any())
                return NotFound("No transactions found for this craftsman.");

            return Ok(transactions);
        }
    }
}
