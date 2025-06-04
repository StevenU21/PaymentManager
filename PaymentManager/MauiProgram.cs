using Microsoft.EntityFrameworkCore;
using PaymentManager.Services;
using PaymentManager.Data;
using CommunityToolkit.Maui;
using PaymentManager.Models;
using PaymentManager.Validators;

namespace PaymentManager
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            builder.Services.AddScoped<IPaymentPlanService, PaymentPlanService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IUserPaymentPlanService, UserPaymentPlanService>();

            builder.Services.AddScoped<IValidationService<UserPaymentPlan>>(provider =>
            {
                var userPaymentPlanService = provider.GetRequiredService<IUserPaymentPlanService>();
                return new ValidationService<UserPaymentPlan>(userPaymentPlanService, new UserPaymentPlanValidator(new List<UserPaymentPlan>()));
            });

            builder.Services.AddScoped<IValidationService<Payment>>(provider =>
            {
                var paymentService = provider.GetRequiredService<IPaymentService>();
                return new ValidationService<Payment>(paymentService, new PaymentValidator(new List<Payment>()));
            });
            builder.Services.AddScoped<IValidationService<User>>(provider =>
            {
                var userService = provider.GetRequiredService<IUserService>();
                return new ValidationService<User>(userService, new UserValidator(new List<User>()));
            });

            builder.Services.AddScoped<IValidationService<PaymentMethod>>(provider =>
            {
                var paymentMethodService = provider.GetRequiredService<IPaymentMethodService>();
                return new ValidationService<PaymentMethod>(paymentMethodService, new PaymentMethodValidator(new List<PaymentMethod>()));
            });

            builder.Services.AddScoped<IValidationService<PaymentPlan>>(provider =>
            {
                var paymentPlanService = provider.GetRequiredService<IPaymentPlanService>();
                return new ValidationService<PaymentPlan>(paymentPlanService, new PaymentPlanValidator(new List<PaymentPlan>()));
            });

            builder.Services.AddSingleton<IMessagingService, MessagingService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                string dbPath = Path.Combine(
                    FileSystem.AppDataDirectory,
                    "source.db"
                );
                options.UseSqlite($"Data Source={dbPath}");
            });
            builder.Services.AddSingleton<AppShell>();

            var app = builder.Build();
            Services = app.Services;

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }
            return app;
        }
    }
}