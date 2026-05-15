using ManagementSystem.Mobile.Services;

namespace ManagementSystem.Mobile;

public partial class App : Application
{
    private readonly IAuthService _authService;

    public App(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell()) { Title = "Management System" };
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await InitializeAppAsync();
    }

    private async Task InitializeAppAsync()
    {
        var hasSession = await _authService.RestoreSessionAsync();
        if (hasSession)
        {
            await Shell.Current.GoToAsync("//main/dashboard");
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
