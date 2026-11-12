using EmergencyFund.Models;
using System.Collections.Concurrent;

namespace EmergencyFund.Repositories
{
    public class InMemoryDepositRepository : IDepositRepository
    {
        private readonly ConcurrentDictionary<Guid, Deposit> _deposits = new();

        public Task<IReadOnlyList<Deposit>> GetAllAsync()
        {
            IReadOnlyList<Deposit> deposits = _deposits.Values
                .OrderBy(d => d.StartDate)
                .ThenBy(d => d.Id)
                .ToList();

            return Task.FromResult(deposits);
        }

        public Task<Deposit?> GetByIdAsync(Guid id)
        {
            if(_deposits.TryGetValue(id, out var deposit))
            {
                return Task.FromResult<Deposit?>(deposit);
            }
            return Task.FromResult<Deposit?>(null);
        }

        public Task<Deposit> AddAsync(Deposit deposit)
        {
            if(_deposits.TryAdd(deposit.Id, deposit))
                return Task.FromResult(deposit);
            throw new ArgumentException("Такой вклад уже существует");
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            return Task.FromResult(_deposits.TryRemove(id, out _));
        }


        public Task<bool> UpdateAsync(Deposit deposit)
        {
            if (_deposits.TryGetValue(deposit.Id, out var current))
            {
                return Task.FromResult(_deposits.TryUpdate(deposit.Id, deposit, current));
            }

            return Task.FromResult(false);
        }
    }
}
