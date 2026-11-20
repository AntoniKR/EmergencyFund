namespace EmergencyFund.Contracts.Goal
{
    public record GoalResponse(
        decimal MonthlyExpenses,
        int Months,
        decimal MonthlySavings,
        decimal TargetAmount,
        decimal CurrentAmount,
        decimal ProgressPercent,
        decimal RemainingAmount,
        int? MonthsToGoal
    );
}
