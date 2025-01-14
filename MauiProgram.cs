using Microsoft.Extensions.Logging;
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
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "sarthakcoursework.db");

                return new DbConnectionService(dbPath);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // Register UserService and its Interface
            builder.Services.AddScoped<IUserService, UserService>();

            // Register TransactionService and its Interface
            builder.Services.AddScoped<ITransactionService, TransactionService>();


            // Register TagService and its Interface
            builder.Services.AddScoped<ITagService, TagService>();

            // MudBlazor Injecting
            builder.Services.AddMudServices();

            return builder.Build();
        }
    }
}
