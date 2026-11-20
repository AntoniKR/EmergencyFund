using EmergencyFund.Contracts.Deposits;

namespace EmergencyFund.Services.Deposits
{
    public interface IDepositService
    {
        Task<IReadOnlyList<DepositResponse>> GetAllAsync();
        Task<DepositResponse?> GetByIdAsync(Guid id);
        Task<DepositResponse> CreateAsync(CreateDepositRequest request);
        Task<DepositResponse?> UpdateAsync(Guid id, UpdateDepositRequest request);
        Task<bool> DeleteAsync(Guid id);

    }
}
