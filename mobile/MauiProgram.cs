using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using ManagementSystem.Mobile.Services;
using ManagementSystem.Mobile.ViewModels;
using ManagementSystem.Mobile.Pages;

namespace ManagementSystem.Mobile;

public static class MauiProgram
{
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

        // Register Services
        var apiBaseUrl = "http://localhost:3000/api/v1";
        builder.Services.AddSingleton<IApiService>(_ => new ApiService(apiBaseUrl));
        builder.Services.AddSingleton<IAuthService, AuthService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<UsersViewModel>();
        builder.Services.AddTransient<DocumentsViewModel>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<DashboardPage>();
        builder.Services.AddTransient<UsersPage>();
        builder.Services.AddTransient<DocumentsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
