using EmergencyFund.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmergencyFund.Contracts.Deposits
{
    public record CreateDepositRequest (
        string Name,
        [Required] DepositType? Type,
        [Required] decimal? Sum,
        [Required] decimal? InterestRate,
        [Required] DateOnly? StartDate,
        int? Term,
        [Required] Capitalization? Capitalization

        );
}
