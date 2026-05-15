using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManagementSystem.Mobile.Models;
using ManagementSystem.Mobile.Services;

namespace ManagementSystem.Mobile.ViewModels;

public partial class DocumentsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private int _currentPage = 1;
    private const int PageSize = 20;

    [ObservableProperty]
    private ObservableCollection<Document> _documents = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasMore = true;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _newDocumentTitle = string.Empty;

    public DocumentsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task InitializeAsync()
    {
        await LoadDocumentsAsync();
    }

    [RelayCommand]
    private async Task LoadDocumentsAsync()
    {
        if (IsLoading) return;

        IsLoading = true;
        _currentPage = 1;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _apiService.GetDocumentsAsync(_currentPage, PageSize);
            Documents = new ObservableCollection<Document>(result.Data);
            HasMore = result.Total > Documents.Count;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load documents: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task CreateDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(NewDocumentTitle))
            return;

        IsLoading = true;
        try
        {
            var document = await _apiService.CreateDocumentAsync(
                NewDocumentTitle,
                $"# {NewDocumentTitle}\n\nStart writing your content here...",
                "markdown");

            Documents.Insert(0, document);
            NewDocumentTitle = string.Empty;

            await Shell.Current.GoToAsync($"documentDetail?id={document.Id}");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to create document: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ViewDocumentAsync(Document document)
    {
        await Shell.Current.GoToAsync($"documentDetail?id={document.Id}");
    }

    [RelayCommand]
    private async Task ExportAsPdfAsync(Document document)
    {
        IsLoading = true;
        try
        {
            var pdfBytes = await _apiService.ExportDocumentAsPdfAsync(document.Id);
            var fileName = $"{document.Title.Replace(" ", "_")}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(filePath, pdfBytes);
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to export PDF: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task ExportAsMarkdownAsync(Document document)
    {
        IsLoading = true;
        try
        {
            var markdown = await _apiService.ExportDocumentAsMarkdownAsync(document.Id);
            var fileName = $"{document.Title.Replace(" ", "_")}.md";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllTextAsync(filePath, markdown);
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to export Markdown: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
