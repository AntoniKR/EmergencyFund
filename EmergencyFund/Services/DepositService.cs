using EmergencyFund.Contracts;
using EmergencyFund.Models;
using EmergencyFund.Repositories;

namespace EmergencyFund.Services
{
    public class DepositService : IDepositService
    {
        private readonly IDepositRepository _repository;

        public DepositService(IDepositRepository repository)
        {
            _repository = repository;
        }
        private static DepositResponse ToResponse(Deposit deposit) => 
            new(deposit.Id, deposit.Name, deposit.Type, deposit.Sum, deposit.InterestRate, deposit.StartDate, deposit.Term,deposit.Capitalization, deposit.EndDate, deposit.EstimatedSum);




        public Task<DepositResponse> CreateAsync(CreateDepositRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<DepositResponse>> GetAllAsync()
        {
            var deposits = await _repository.GetAllAsync();
            var result = deposits.Select(ToResponse).ToList();
            return result;       
        }

        public async Task<DepositResponse?> GetByIdAsync(Guid id)
        {
            var deposit = await _repository.GetByIdAsync(id);
            if(deposit is null)
            {
                return null;
            }
            return ToResponse(deposit);
        }

        public Task<DepositResponse?> UpdateAsync(Guid id, UpdateDepositRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
