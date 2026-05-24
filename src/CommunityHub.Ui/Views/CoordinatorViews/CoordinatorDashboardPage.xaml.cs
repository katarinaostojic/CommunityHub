using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CoordinatorDashboardPage : Page
{
    private readonly long _userId;

    public CoordinatorDashboardPage(long userId)
    {
        InitializeComponent();
        _userId = userId;
    }

    private void MyDistrictsButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_userId), "My Districts");
    }

    private void RequestsButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new ManageRequestsPage(_userId), "Requests");
    }

    private void ForumsButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new ForumsPage(_userId), "Forums");
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        CoordinatorMainWindow.Instance.Close();
    }
}