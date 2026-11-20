using EmergencyFund.Models;

namespace EmergencyFund.Repositories.DepositRepository
{
    public interface IDepositRepository
    {
        Task<IReadOnlyList<Deposit>> GetAllAsync(); // Получить список депозитов
        Task<Deposit?> GetByIdAsync(Guid id);
        Task<Deposit> AddAsync(Deposit deposit);
        Task<bool> UpdateAsync(Deposit deposit);
        Task<bool> DeleteAsync(Guid id);    // возвращаем true или false при удалении и все
    }
}
 