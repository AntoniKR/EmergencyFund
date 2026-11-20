using EmergencyFund.Models;
using EmergencyFund.Repositories.GoalRepository;

namespace EmergencyFund.Repositories
{
    public class InMemoryGoalRepository : IGoalRepository
    {
        private Goal? _goal;
        public Task<Goal?> GetAsync()
        {            
            return Task.FromResult(_goal);
        }

        public Task<Goal> SetGoalAsync(Goal goal)
        {
            _goal = goal;
            return Task.FromResult(_goal);
        }

        public Task<bool> DeleteGoalAsync()
        {

            Goal? oldGoal = Interlocked.Exchange(ref _goal, null);

            // Если старая цель и так была Null, то возвращаем false для обработки в сервисе с ошибкой 404
            return Task.FromResult(oldGoal is not null);
        }
    }
}
