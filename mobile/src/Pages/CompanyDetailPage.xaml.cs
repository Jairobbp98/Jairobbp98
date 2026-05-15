namespace ManagementSystem.Mobile.Pages;

[QueryProperty(nameof(CompanyId), "id")]
public partial class CompanyDetailPage : ContentPage
{
    public string CompanyId { get; set; } = string.Empty;

    public CompanyDetailPage()
    {
        InitializeComponent();
    }
}
