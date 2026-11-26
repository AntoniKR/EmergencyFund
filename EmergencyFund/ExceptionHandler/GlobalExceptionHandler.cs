using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EmergencyFund.ExceptionHandler
{
    public sealed class GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext, Exception exception, CancellationToken cancelToken)
        {
            ProblemDetails problem;

            if (exception is BusinessRuleException ruleException)
            {
                logger.LogInformation("Нарушено бизнес-правило: {Field} - {Message}", ruleException.Field, ruleException.Message);

                var errors = new Dictionary<string, string[]>
                {
                    [ruleException.Field] = new[] { ruleException.Message }
                };

                problem = new ValidationProblemDetails(errors)
                {
                    Status = StatusCodes.Status400BadRequest
                };
            }
            else
            {
                logger.LogError(exception, "Необработанное исключение");

                problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Внутренняя ошибка сервера"
                };
            }

            httpContext.Response.StatusCode = problem.Status!.Value;
            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = exception
            });



        }

    }
}
