namespace EmergencyFund.Services.Goals
{
    public static class GoalCalculator
    {
        // Вычисление суммы цели
        public static decimal CalculateTarget(decimal monthlyExpenses, int months) => monthlyExpenses * months;

        public static decimal CalculateProgressPercent(decimal currentAmount, decimal targetAmount) => 
            Math.Round((currentAmount / targetAmount * 100), 2, MidpointRounding.AwayFromZero);

        public static decimal CalculateRemainingAmount(decimal targetAmount, decimal currentAmount) =>
            Math.Max(0, targetAmount - currentAmount);

        public static int? CalculateMonthsToGoal(decimal remainingAmount, decimal monthlySavings)
        {
            if (remainingAmount == 0)   // Если оставшаяся сумма 0, то все накоплено
                return 0;
            if (monthlySavings == 0) // Если не копит, то неизвестно, через сколько месяцев дойдет до цели
                return null;

            int result = (int)Math.Ceiling(remainingAmount / monthlySavings);
            return result;            
        }

    }
}
