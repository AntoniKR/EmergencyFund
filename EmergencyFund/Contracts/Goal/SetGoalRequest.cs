using System.ComponentModel.DataAnnotations;

namespace EmergencyFund.Contracts.Goal
{
    public record SetGoalRequest (
        [Required] decimal? MonthlyExpenses,
        [Required] int? Months,
        [Required] decimal? MonthlySavings
    );
}
