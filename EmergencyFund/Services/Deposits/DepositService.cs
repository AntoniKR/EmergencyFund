using EmergencyFund.Contracts.Deposits;
using EmergencyFund.ExceptionHandler;
using EmergencyFund.Models;
using EmergencyFund.Options;
using EmergencyFund.Repositories.DepositRepository;
using Microsoft.Extensions.Options;

namespace EmergencyFund.Services.Deposits
{
    public class DepositService : IDepositService
    {
        private readonly IDepositRepository _repository;
        private readonly DepositCalculationOptions _options;
        public DepositService(IDepositRepository repository,
            IOptionsSnapshot<DepositCalculationOptions> options)
        {
            _repository = repository;
            _options = options.Value;
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
            deposit.EstimatedSum = DepositCalculator.CalculateEstimatedSum(deposit.Sum, deposit.InterestRate, deposit.StartDate, deposit.Term, deposit.Capitalization, _options.DaysInYear);

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
            var deposit = await _repository.GetByIdAsync(id);
            if (deposit is null)
                return null;
            if (deposit.Type != Models.Enums.DepositType.SavingsAccount && request.Sum is not null)
                throw new BusinessRuleException(nameof(request.Sum), "Это вклад, а не накопительный счёт. Поменять нельзя");
            var newDeposit = new Deposit
            {
                Id = deposit.Id,
                Name = request.Name ?? deposit.Name,
                Type = deposit.Type,
                Sum = request.Sum ?? deposit.Sum,
                InterestRate = deposit.InterestRate,
                StartDate = deposit.StartDate,
                Term = deposit.Term,
                Capitalization = deposit.Capitalization,
                EndDate = deposit.EndDate
            };

            newDeposit.EstimatedSum = DepositCalculator.CalculateEstimatedSum(newDeposit.Sum, newDeposit.InterestRate, newDeposit.StartDate, newDeposit.Term, newDeposit.Capitalization, _options.DaysInYear);

            var result = await _repository.UpdateAsync(newDeposit);
            if (!result)
                return null;
            return ToResponse(newDeposit);
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
