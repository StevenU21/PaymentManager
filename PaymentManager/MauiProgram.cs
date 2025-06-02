using Microsoft.EntityFrameworkCore;
using PaymentManager.Services;
using PaymentManager.Data; 
using CommunityToolkit.Maui;
using PaymentManager.Models;
using PaymentManager.Validators; 
using System.IO;

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
            builder.Services.AddScoped<IPaymentTypeService, PaymentTypeService>();
            builder.Services.AddScoped<IValidationService<User>>(provider =>
            {
                var userService = provider.GetRequiredService<IUserService>();
                return new ValidationService<User>(userService, new UserValidator(new List<User>()));
            });

            builder.Services.AddScoped<IValidationService<PaymentType>>(provider =>
            {
                var paymentTypeService = provider.GetRequiredService<IPaymentTypeService>();
                return new ValidationService<PaymentType>(paymentTypeService, new PaymentTypeValidator(new List<PaymentType>()));
            });

            builder.Services.AddSingleton<IMessagingService, MessagingService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                string dbPath = Path.Combine(
                    FileSystem.AppDataDirectory, 
                    "app.db"
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
