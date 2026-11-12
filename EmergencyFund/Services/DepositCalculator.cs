using EmergencyFund.Models.Enums;


namespace EmergencyFund.Services
{
    public static class DepositCalculator
    {
        public static DateOnly? CalculateEndDate(DateOnly startDate, int? termMonths)
        {
            if (termMonths is null)
            {
                return null;
            }
            return startDate.AddMonths(termMonths.Value);

            // Это удобнее,но пока тяжело для глаза
            //return termMonths is not null ? startDate.AddMonths(termMonths.Value) : null;   // Либо такой вариант  
        }


        public static decimal? CalculateEstimatedSum(decimal sum, decimal interestRate, DateOnly startDate, int? termMonths, Capitalization capitalization)
        {
            if (termMonths is null)
            {
                return null;
            }

            decimal annualRate = interestRate / 100;
            decimal dailyRate = annualRate / 365;
            decimal estimatedSum = sum;

            switch (capitalization)
            {
                case Capitalization.None:
                    {
                        estimatedSum = sum * (1 + (annualRate * ((decimal)termMonths.Value / 12)));
                        return Math.Round(estimatedSum, 2, MidpointRounding.AwayFromZero);
                    }
                case Capitalization.Daily:
                    {
                        DateOnly endDate = CalculateEndDate(startDate, termMonths).Value;
                        int countDays = endDate.DayNumber - startDate.DayNumber;

                        for(int i = 0; i < countDays; i++)
                        {
                            estimatedSum = estimatedSum * (1 + dailyRate);
                        }
                        return Math.Round(estimatedSum, 2, MidpointRounding.AwayFromZero);
                    }
                case Capitalization.Weekly:
                    {
                        decimal weeklyRate = annualRate * 7 / 365;

                        DateOnly endDate = CalculateEndDate(startDate, termMonths).Value;
                        int countDays = (endDate.DayNumber - startDate.DayNumber);
                        int countWeeks = countDays / 7;
                        int remainingDays = countDays % 7;

                        for (int i = 0; i < countWeeks; i++)
                        {
                            estimatedSum = estimatedSum * (1 + weeklyRate);
                        }
                        for (int i = 0; i < remainingDays; i++)
                        {
                            estimatedSum = estimatedSum * (1 + dailyRate);
                        }
                        return Math.Round(estimatedSum, 2, MidpointRounding.AwayFromZero);
                    }
                case Capitalization.Monthly:
                    {
                        for (int i = 0; i < termMonths; i++)
                        {
                            estimatedSum = estimatedSum * (1 + (annualRate / 12));
                        }
                        return Math.Round(estimatedSum, 2, MidpointRounding.AwayFromZero);
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(capitalization), capitalization, "Unknown capitalization type");
                    }
            }    
            
        }
    }
}
