using EmergencyFund.Models.Enums;

namespace EmergencyFund.Models
{
    public class Deposit
    {
        public Guid Id { get; set; } = Guid.NewGuid();  // Id
        public required string Name { get; set; }       // Название вклада
        public DepositType Type { get; set; }           // Тип вклада
        public decimal Sum { get; set; }                // Сумма вклада
        public decimal InterestRate { get; set; }       // Ставка вклада
        public DateOnly StartDate { get; set; }         // Начало вклада
        public int? Term { get; set; }                  // Срок, null, если накопительный счет
        public Capitalization Capitalization { get; set; }   // Период капитализации
        public DateOnly? EndDate { get; set; }          // Конец вклада (если не накопительный счет)
        public decimal? EstimatedSum { get; set; }      // Сумма в конце срока (если не накопительный счет)
    }
}
