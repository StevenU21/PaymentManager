using Microsoft.EntityFrameworkCore;
using PaymentManager.Services; 
using PaymentManager.Views; 

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddDbContext<Data.AppDbContext>(options =>
            {
                options.UseSqlite("Data Source=app.db"); 
            });
            builder.Services.AddSingleton<AppShell>();

            var app = builder.Build();
            Services = app.Services;

            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
                db.Database.EnsureCreated();
            }
            return app;
        }
    }
}
