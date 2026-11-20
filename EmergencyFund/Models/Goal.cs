namespace EmergencyFund.Models
{
    public class Goal
    {
        public decimal MonthlyExpenses { get; set; }    // Траты в месяц
        public int Months { get; set; } // Кол-во месяцев
        public decimal MonthlySavings { get; set; } // Сколько откладывает в месяц
    }
}
