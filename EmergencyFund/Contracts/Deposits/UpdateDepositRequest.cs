using System.ComponentModel.DataAnnotations;

namespace EmergencyFund.Contracts.Deposits
{
    public record UpdateDepositRequest(
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Название депозита не может быть пустым")] string? Name,
        [Range(0, double.MaxValue, MinimumIsExclusive = true, ErrorMessage = "Сумма должна быть больше 0")] decimal? Sum 
    ) : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Name is null && Sum is null)
            {
                yield return new ValidationResult(
                    "Укажите хотя бы одно поле: name или sum");
            }
            if(Name is not null && string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult(
                    "Необходимо указать корректное новое имя, состоящее из букв",
                    new[] { nameof(Name) });
            }
        }
    }
}
