using EmergencyFund.Models;

namespace EmergencyFund.Repositories.GoalRepository
{
    public interface IGoalRepository
    {
        Task<Goal?> GetAsync();
        Task<Goal> SetGoalAsync(Goal goal);
        Task<bool> DeleteGoalAsync();
    }
}
 