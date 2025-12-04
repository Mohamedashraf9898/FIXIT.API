using FIXIT.BLL.DTOs.NotificationDtos;
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
        private readonly INotificationService _notificationService;  // ✅ ADD THIS

        // ✅ ADD INotificationService to constructor
        public WalletController(IWalletService walletService, INotificationService notificationService)
        {
            _walletService = walletService;
            _notificationService = notificationService;  // ✅ ADD THIS
        }

        // ✅ CHANGE: Add "craftsman/" to route
        [HttpGet("craftsman/{craftsManId}")]
        public async Task<IActionResult> GetWallet(int craftsManId)
        {
            try
            {
                var wallet = await _walletService.GetWalletAsync(craftsManId);
                return Ok(wallet);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddFunds([FromBody] CreateWalletTransactionDto dto)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawFunds([FromBody] CreateWalletTransactionDto dto)
        {
            var result = await _walletService.WithdrawFundsAsync(dto);

            // Send notification to Admin with CraftsManId
            var notificationDto = new CreateNotificationDto
            {
                //ServiceRequestId = null,  // ✅ Withdrawal doesn't need service request
                CraftsManId = dto.CraftsManId,  // ✅ Add craftsman ID
                Title = "New Withdrawal Request",
                Message = $"Craftsman requested withdrawal of {dto.Amount} EGP via {dto.Transactiontype}",
                FinalAmount = dto.Amount,
                Description = dto.TransationInfo,
                Type = NotificationType.WithdrawalRequested
                //RecipientType = "Admin"  // ✅ This is for admin
            };

            await _notificationService.CreateForAdminAsync(notificationDto);

            return Ok(result);
        }

        // ✅ CHANGE: Add "craftsman/" to route
        [HttpGet("craftsman/{craftsManId}/transactions")]
        public async Task<IActionResult> GetTransactions(int craftsManId)
        {
            try
            {
                var transactions = await _walletService.GetWalletTransactionsAsync(craftsManId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // ✅ ADD: Proper HTTP endpoint with [HttpPut]
        [HttpPut("transaction")]
        public async Task<IActionResult> UpdateWalletTransaction([FromBody] UpdateWaletTransactionDto dto)
        {
            try
            {
                var result = await _walletService.UpdateWaletTransaction(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}