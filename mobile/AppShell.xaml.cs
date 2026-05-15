using ManagementSystem.Mobile.Pages;

namespace ManagementSystem.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("userDetail", typeof(UserDetailPage));
        Routing.RegisterRoute("documentDetail", typeof(DocumentDetailPage));
        Routing.RegisterRoute("companyDetail", typeof(CompanyDetailPage));
    }
}
