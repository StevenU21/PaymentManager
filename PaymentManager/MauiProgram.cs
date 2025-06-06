using Microsoft.EntityFrameworkCore;
using PaymentManager.Services;
using PaymentManager.Data;
using CommunityToolkit.Maui;
using PaymentManager.Models;
using PaymentManager.Validators;
using Syncfusion.Licensing;
using System.Text.Json;

namespace PaymentManager
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; } = null!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            string licenseKey = LoadSyncfusionLicenseKey();
            if (!string.IsNullOrWhiteSpace(licenseKey))
            {
                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Syncfusion License Key not found or invalid.");
            }
            
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
            builder.Services.AddScoped<IBaseService<UserPaymentPlan>>(provider => provider.GetRequiredService<IUserPaymentPlanService>());

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
                    "nose.db"
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

        private static string LoadSyncfusionLicenseKey()
        {
            try
            {
                // Buscar el archivo en la raíz y en la carpeta de salida
                string[] possiblePaths = new[] {
                    Path.Combine(AppContext.BaseDirectory, "syncfusion.license.json"),
                    Path.Combine(Environment.CurrentDirectory, "syncfusion.license.json"),
                    Path.Combine(Directory.GetCurrentDirectory(), "syncfusion.license.json")
                };
                foreach (var licenseFile in possiblePaths)
                {
                    if (File.Exists(licenseFile))
                    {
                        var json = File.ReadAllText(licenseFile);
                        using var doc = JsonDocument.Parse(json);
                        if (doc.RootElement.TryGetProperty("SyncfusionLicenseKey", out var keyProp))
                        {
                            return keyProp.GetString() ?? string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error leyendo Syncfusion License: {ex.Message}");
            }
            return string.Empty;
        }
    }
}