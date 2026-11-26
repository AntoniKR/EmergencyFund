using EmergencyFund.ExceptionHandler;
using EmergencyFund.Options;
using EmergencyFund.Repositories;
using EmergencyFund.Repositories.DepositRepository;
using EmergencyFund.Repositories.GoalRepository;
using EmergencyFund.Services.Deposits;
using EmergencyFund.Services.Goals;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmergencyFund
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Перевод чисел в строки enums для JSON. Например, monthly пишется числом, а с этой настройкой будет monthly
                    options.JsonSerializerOptions.Converters.Add(
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                    // Если неизвестные поля то ошибка 400, т.е. не тихая ошибка
                    options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
                });



            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<IDepositService, DepositService>();
            builder.Services.AddSingleton<IDepositRepository, InMemoryDepositRepository>();
            builder.Services.AddScoped<IGoalService, GoalService>();
            builder.Services.AddSingleton<IGoalRepository, InMemoryGoalRepository>();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Logging.AddSimpleConsole(options => options.IncludeScopes = true);

            builder.Services.AddOptions<DepositCalculationOptions>()
                .BindConfiguration(DepositCalculationOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var app = builder.Build();
            Console.WriteLine($"DaysInYear = {app.Configuration["DepositCalculation:DaysInYear"]}");
            app.UseExceptionHandler();  // Для обработки всяких ошибок с помощью класса GlobalExceptionHandler

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
