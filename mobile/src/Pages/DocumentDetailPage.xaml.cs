namespace ManagementSystem.Mobile.Pages;

[QueryProperty(nameof(DocumentId), "id")]
public partial class DocumentDetailPage : ContentPage
{
    public string DocumentId { get; set; } = string.Empty;

    public DocumentDetailPage()
    {
        InitializeComponent();
    }
}
