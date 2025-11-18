using FIXIT.BLL.DTOs.WithdrawalDTOs;
using FIXIT.BLL.Services.IService.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FIXIT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WithdrawalsController : ControllerBase
    {
        private readonly IWithdrawalService _withdrawalService;

        public WithdrawalsController(IWithdrawalService withdrawalService)
        {
            _withdrawalService = withdrawalService;
        }

        // Craftsman sends withdrawal request
        [Authorize(Roles = "CraftsMan")]
        [HttpPost("request")]
        public async Task<IActionResult> Create([FromBody] WithdrawalRequestDto dto)
        {
            int craftsManId = int.Parse(User.FindFirst("Id")!.Value);

            var result = await _withdrawalService.CreateWithdrawalAsync(craftsManId, dto);
            return Ok(result);
        }

        // Admin views all pending withdrawals
        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _withdrawalService.GetPendingWithdrawalsAsync();
            return Ok(result);
        }

        // Admin approves a pending withdrawal
        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _withdrawalService.ApproveAsync(id);
            return Ok(result);
        }
    }
}
