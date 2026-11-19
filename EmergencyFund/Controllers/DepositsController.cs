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
        public async Task<ActionResult<IReadOnlyList<DepositResponse>>> GetAll()
        {
            var result = await _depositService.GetAllAsync();
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<DepositResponse>> GetById(Guid id)
        {
            var result = await _depositService.GetByIdAsync(id);
            if(result is null)
                return NotFound();

            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<DepositResponse>> Create(CreateDepositRequest request)
        {
            var deposit = await _depositService.CreateAsync(request);

            return CreatedAtAction(nameof(GetById), new { id  = deposit.Id }, deposit);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool result = await _depositService.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
