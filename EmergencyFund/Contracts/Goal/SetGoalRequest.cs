using System.ComponentModel.DataAnnotations;

namespace EmergencyFund.Contracts.Goal
{
    public record SetGoalRequest (
        [Required][Range(0, double.MaxValue, MinimumIsExclusive = true, ErrorMessage = "Траты в месяц должны быть больше нуля")] decimal? MonthlyExpenses,
        [Required][Range(1, int.MaxValue, ErrorMessage = "Цель должна быть минимум на один месяц")] int? Months,
        [Required][Range(0, double.MaxValue, ErrorMessage = "Накопления в месяц должны быть не меньше нуля")] decimal? MonthlySavings
    );
}
