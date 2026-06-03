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

    private void CityObjectsButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new CityObjectsPage(_userId), "City Objects");
    }

    private void ReviewsButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new NeighborhoodReviewsPage(_userId), "Reviews");
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        CoordinatorMainWindow.Instance.Close();
    }
    private void BudgetButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new BudgetPage(_userId), "Budget");
    }
    private void CityGiftsButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new CityGiftsPage(_userId), "City Gifts");
    }
}