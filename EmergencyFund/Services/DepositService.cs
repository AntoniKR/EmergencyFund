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

        public async Task<DepositResponse> CreateAsync(CreateDepositRequest request)
        {
            var deposit = new Deposit
            {
                Name = request.Name,
                Type = request.Type.Value,
                Sum = request.Sum.Value,
                InterestRate = request.InterestRate.Value,
                StartDate = request.StartDate.Value,
                Term = request.Term,
                Capitalization = request.Capitalization.Value            
            };

            // Вычисляем конец даты вклада и ожидаемую сумму
            deposit.EndDate = DepositCalculator.CalculateEndDate(deposit.StartDate, deposit.Term);
            deposit.EstimatedSum = DepositCalculator.CalculateEstimatedSum(deposit.Sum, deposit.InterestRate, deposit.StartDate, deposit.Term, deposit.Capitalization);

            var result = await _repository.AddAsync(deposit);
            return ToResponse(result);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);         
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

        public async Task<DepositResponse?> UpdateAsync(Guid id, UpdateDepositRequest request)
        {
            throw new NotImplementedException();
        }

        private static DepositResponse ToResponse(Deposit deposit) => new
            (deposit.Id,
            deposit.Name,
            deposit.Type,
            deposit.Sum,
            deposit.InterestRate,
            deposit.StartDate,
            deposit.Term,
            deposit.Capitalization,
            deposit.EndDate,
            deposit.EstimatedSum);

    }
}
