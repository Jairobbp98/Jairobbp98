using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagementSystem.Mobile.Models;
using ManagementSystem.Mobile.Services;

namespace ManagementSystem.Mobile.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IAuthService _authService;
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _welcomeMessage = string.Empty;

    [ObservableProperty]
    private int _totalUsers;

    [ObservableProperty]
    private int _totalCompanies;

    [ObservableProperty]
    private int _totalDocuments;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private ObservableCollection<Document> _recentDocuments = new();

    public DashboardViewModel(IAuthService authService, IApiService apiService)
    {
        _authService = authService;
        _apiService = apiService;
    }

    public async Task InitializeAsync()
    {
        if (_authService.CurrentUser != null)
        {
            WelcomeMessage = $"Welcome back, {_authService.CurrentUser.FirstName}!";
        }

        await LoadDashboardDataAsync();
    }

    [RelayCommand]
    private async Task LoadDashboardDataAsync()
    {
        IsLoading = true;
        try
        {
            var usersTask = _apiService.GetUsersAsync(1, 1);
            var companiesTask = _apiService.GetCompaniesAsync(1, 1);
            var documentsTask = _apiService.GetDocumentsAsync(1, 5);

            await Task.WhenAll(usersTask, companiesTask, documentsTask);

            TotalUsers = (await usersTask).Total;
            TotalCompanies = (await companiesTask).Total;

            var docsResult = await documentsTask;
            TotalDocuments = docsResult.Total;
            RecentDocuments = new ObservableCollection<Document>(docsResult.Data);
        }
        catch (Exception ex)
        {
            // Log error - in production this would use a proper logging service
            System.Diagnostics.Debug.WriteLine($"Dashboard load error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToUsersAsync()
    {
        await Shell.Current.GoToAsync("users");
    }

    [RelayCommand]
    private async Task NavigateToCompaniesAsync()
    {
        await Shell.Current.GoToAsync("companies");
    }

    [RelayCommand]
    private async Task NavigateToDocumentsAsync()
    {
        await Shell.Current.GoToAsync("documents");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }
}
