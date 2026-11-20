using EmergencyFund.Contracts.Goal;
using EmergencyFund.Models;
using EmergencyFund.Services.Goals;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyFund.Controllers
{
    [ApiController]
    [Route("api/goal")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _goalService;
        public GoalController(IGoalService goalService)
        {
            _goalService = goalService;
        }
        [HttpGet]
        public async Task<ActionResult<GoalResponse>> Get()
        {
            var result = await _goalService.GetAsync();
            if (result is null)
                return NotFound(); 
            return result;
        }
        [HttpPut]
        public async Task<ActionResult<GoalResponse>> Set(SetGoalRequest request)
        {
            var goal = await _goalService.SetAsync(request);
            return goal;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            bool result = await _goalService.DeleteAsync();
            return result ? NoContent() : NotFound();
        }
    }
}
