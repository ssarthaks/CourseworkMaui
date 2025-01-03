using Microsoft.Extensions.Logging;
using ExpenwiseTracker.Model;  // Add the necessary namespaces
using System.IO;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using ExpenwiseTracker.Services;
using ExpenwiseTracker.Services.Interface;
using MudBlazor.Services;

namespace ExpenwiseTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Register Blazor WebView
            builder.Services.AddMauiBlazorWebView();

            // Register DatabaseService with the database file path
            builder.Services.AddScoped<DbConnectionService>(serviceProvider =>
            {
                // Specify the database file path
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "expenwisetracker.db");

                // Return a new instance of DatabaseService with the dbPath
                return new DbConnectionService(dbPath);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Register IUserService and its implementation UserService
            builder.Services.AddScoped<IUserService, UserService>();

            // Register TransactionService (if needed, add it like this)
            builder.Services.AddScoped<TransactionService>();
            builder.Services.AddSingleton<TagService>();

            //MudBlazor Injecting
            builder.Services.AddMudServices();

            return builder.Build();
        }
    }
}
