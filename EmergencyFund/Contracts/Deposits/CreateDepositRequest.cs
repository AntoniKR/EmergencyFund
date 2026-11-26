using EmergencyFund.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmergencyFund.Contracts.Deposits
{
    public record CreateDepositRequest(
            [Required][StringLength(50, MinimumLength = 1, ErrorMessage = "Название депозита не может быть пустым")] string Name,
            [Required] DepositType? Type,

            // MinimumIsExclusive означает строго больше минимального значения (нуля)
            [Required][Range(0, double.MaxValue, MinimumIsExclusive = true, ErrorMessage = "Сумма должна быть больше 0")] decimal? Sum,

            [Required][Range(0, 100, MinimumIsExclusive = true, ErrorMessage = "Ставка должна быть больше 0 и меньше 100")] decimal? InterestRate,
            [Required] DateOnly? StartDate,
            int? Term,
            [Required] Capitalization? Capitalization
            ) : IValidatableObject
    {        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Type == DepositType.Deposit)
            {
                if (Term is null)
                {
                    yield return new ValidationResult(
                        "Необходимо указать срок вклада",
                        new[] { nameof(Term) });
                }
                else if (Term <= 0)
                {
                    yield return new ValidationResult(
                        "Необходимо указать срок вклада больше 0",
                        new[] { nameof(Term) });
                }
            }
            else
            {
                if (Term is not null)
                {
                    yield return new ValidationResult(
                        "Нельзя указать срок накопительного счёта",
                        new[] { nameof(Term) });
                }
            }

            if (StartDate > DateOnly.FromDateTime(DateTime.Today))
            {
                yield return new ValidationResult(
                    "Нельзя указать дату вклада или накопительного счёта позже сегодняшнего дня",
                    new[] { nameof(StartDate) });
            }
        }
    }
}
