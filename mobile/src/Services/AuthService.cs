using ManagementSystem.Mobile.Models;

namespace ManagementSystem.Mobile.Services;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    UserProfile? CurrentUser { get; }
    string? AccessToken { get; }
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string firstName, string lastName, string password);
    Task LogoutAsync();
    Task<bool> RestoreSessionAsync();
}

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;

    private const string AccessTokenKey = "access_token";
    private const string RefreshTokenKey = "refresh_token";
    private const string UserIdKey = "user_id";
    private const string UserEmailKey = "user_email";
    private const string UserFirstNameKey = "user_first_name";
    private const string UserLastNameKey = "user_last_name";
    private const string UserRoleKey = "user_role";

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
    public UserProfile? CurrentUser { get; private set; }
    public string? AccessToken { get; private set; }

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var result = await _apiService.LoginAsync(email, password);
            await SaveSessionAsync(result);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(
        string email, string firstName, string lastName, string password)
    {
        try
        {
            var result = await _apiService.RegisterAsync(email, firstName, lastName, password);
            await SaveSessionAsync(result);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        CurrentUser = null;
        AccessToken = null;
        _apiService.ClearAuthToken();

        await SecureStorage.Default.SetAsync(AccessTokenKey, string.Empty);
        await SecureStorage.Default.SetAsync(RefreshTokenKey, string.Empty);
        Preferences.Default.Clear();
    }

    public async Task<bool> RestoreSessionAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(AccessTokenKey);
            if (string.IsNullOrEmpty(token))
                return false;

            AccessToken = token;
            _apiService.SetAuthToken(token);

            CurrentUser = new UserProfile
            {
                Id = Preferences.Default.Get(UserIdKey, string.Empty),
                Email = Preferences.Default.Get(UserEmailKey, string.Empty),
                FirstName = Preferences.Default.Get(UserFirstNameKey, string.Empty),
                LastName = Preferences.Default.Get(UserLastNameKey, string.Empty),
                Role = Preferences.Default.Get(UserRoleKey, string.Empty),
            };

            return !string.IsNullOrEmpty(CurrentUser.Id);
        }
        catch
        {
            return false;
        }
    }

    private async Task SaveSessionAsync(AuthResult result)
    {
        AccessToken = result.AccessToken;
        CurrentUser = result.User;
        _apiService.SetAuthToken(result.AccessToken);

        await SecureStorage.Default.SetAsync(AccessTokenKey, result.AccessToken);
        await SecureStorage.Default.SetAsync(RefreshTokenKey, result.RefreshToken);

        Preferences.Default.Set(UserIdKey, result.User.Id);
        Preferences.Default.Set(UserEmailKey, result.User.Email);
        Preferences.Default.Set(UserFirstNameKey, result.User.FirstName);
        Preferences.Default.Set(UserLastNameKey, result.User.LastName);
        Preferences.Default.Set(UserRoleKey, result.User.Role);
    }
}
