namespace ManagementSystem.Mobile.Pages;

[QueryProperty(nameof(UserId), "id")]
public partial class UserDetailPage : ContentPage
{
    public string UserId { get; set; } = string.Empty;

    public UserDetailPage()
    {
        InitializeComponent();
    }
}
