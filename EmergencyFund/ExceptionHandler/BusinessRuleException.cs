namespace EmergencyFund.ExceptionHandler
{
    public class BusinessRuleException : Exception
    {
        public string Field { get; }

        public BusinessRuleException(string field, string message) : base(message)
        {
            Field = field;
        }
    }
}
