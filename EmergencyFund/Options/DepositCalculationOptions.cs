using System.ComponentModel.DataAnnotations;

namespace EmergencyFund.Options
{
    public class DepositCalculationOptions
    {
        public const string SectionName = "DepositCalculation";

        [Range(360, 366)]
        public int DaysInYear { get; set; } = 365;
    }
}
