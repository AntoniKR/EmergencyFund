using EmergencyFund.Contracts.Goal;
using EmergencyFund.Models;
using EmergencyFund.Repositories.DepositRepository;
using EmergencyFund.Repositories.GoalRepository;

namespace EmergencyFund.Services.Goals
{
    public class GoalService : IGoalService
    {
        private readonly IDepositRepository _repoDeposits;
        private readonly IGoalRepository _repoGoal;
        public GoalService(IDepositRepository repoDeposits, IGoalRepository repoGoal)
        {
            _repoDeposits = repoDeposits;
            _repoGoal = repoGoal;
        }      

        public async Task<GoalResponse?> GetAsync()
        {
            var goal = await _repoGoal.GetAsync();
            if(goal is null)
            {
                return null;
            }
            decimal currentAmount = await GetCurrentAmountAsync();
            return ToResponse(goal, currentAmount);
        }

        public async Task<GoalResponse> SetAsync(SetGoalRequest request)
        {
            Goal goal = new Goal
            {
                MonthlyExpenses = request.MonthlyExpenses.Value,
                Months = request.Months.Value,
                MonthlySavings = request.MonthlySavings.Value
            };
            await _repoGoal.SetGoalAsync(goal);
            decimal currentAmount = await GetCurrentAmountAsync();
            return ToResponse(goal, currentAmount);
        }

        public async Task<bool> DeleteAsync()
        {
            return await _repoGoal.DeleteGoalAsync();
        }

        private async Task<decimal> GetCurrentAmountAsync()
        {
            IReadOnlyList<Deposit> sumDeposits = await _repoDeposits.GetAllAsync();
            decimal currentAmount = sumDeposits.Sum(s => s.Sum);
            return currentAmount;
        }

        private static GoalResponse ToResponse(Goal goal, decimal currentAmount)
        {
            var targetAmount = GoalCalculator.CalculateTarget(goal.MonthlyExpenses, goal.Months);
            var remainingAmount = GoalCalculator.CalculateRemainingAmount(targetAmount, currentAmount);

            return new GoalResponse(
                goal.MonthlyExpenses,
                goal.Months,
                goal.MonthlySavings,
                targetAmount,
                currentAmount,
                GoalCalculator.CalculateProgressPercent(currentAmount, targetAmount),
                remainingAmount,
                GoalCalculator.CalculateMonthsToGoal(remainingAmount, goal.MonthlySavings)
            );
        }
    }
}
