using EmergencyFund.Models.Enums;


namespace EmergencyFund.Contracts
{
    public record DepositResponse(
    Guid Id,
    string Name,
    DepositType Type,
    decimal Sum,
    decimal InterestRate,
    DateOnly StartDate,
    int? Term,
    Capitalization Capitalization,
    DateOnly? EndDate,
    decimal? EstimatedSum
    );
}
