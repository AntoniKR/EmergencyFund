namespace EmergencyFund.Contracts.Deposits
{
    public record UpdateDepositRequest(
        string? Name,
        decimal? Sum // как написать условие, что если deposittype накопительный счет?
    );
}
