using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagementSystem.Mobile.Models;
using ManagementSystem.Mobile.Services;

namespace ManagementSystem.Mobile.ViewModels;

public partial class UsersViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private int _currentPage = 1;
    private const int PageSize = 20;

    [ObservableProperty]
    private ObservableCollection<User> _users = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasMore = true;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public UsersViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task InitializeAsync()
    {
        await LoadUsersAsync();
    }

    [RelayCommand]
    private async Task LoadUsersAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        _currentPage = 1;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _apiService.GetUsersAsync(_currentPage, PageSize);
            Users = new ObservableCollection<User>(result.Data);
            HasMore = result.Total > Users.Count;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load users: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsLoading || !HasMore) return;

        IsLoading = true;
        _currentPage++;

        try
        {
            var result = await _apiService.GetUsersAsync(_currentPage, PageSize);
            foreach (var user in result.Data)
            {
                Users.Add(user);
            }
            HasMore = result.Total > Users.Count;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load more users: {ex.Message}";
            _currentPage--;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewUserDetailsAsync(User user)
    {
        await Shell.Current.GoToAsync($"userDetail?id={user.Id}");
    }
}
