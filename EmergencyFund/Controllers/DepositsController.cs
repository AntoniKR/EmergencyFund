using EmergencyFund.Contracts;
using EmergencyFund.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyFund.Controllers
{
    [ApiController]
    [Route("api/deposits")]
    public class DepositsController : ControllerBase
    {
        private readonly IDepositService _depositService;
        public DepositsController(IDepositService depositService)
        {
            _depositService = depositService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListDeposits()
        {
            var result = await _depositService.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeposit(Guid id)
        {
            var result = await _depositService.GetByIdAsync(id);
            if(result is null)
                return NotFound();

            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateDepositRequest request)
        {
            var deposit = await _depositService.CreateAsync(request);

            return CreatedAtAction(nameof(GetDeposit), new { id  = deposit.Id }, deposit);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool result = await _depositService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
