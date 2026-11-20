using EmergencyFund.Contracts.Goal;

namespace EmergencyFund.Services.Goals
{
    public interface IGoalService
    {
        Task<GoalResponse> GetAsync();
        Task<GoalResponse> SetAsync(SetGoalRequest request);
        Task<bool> DeleteAsync();
    }
}
