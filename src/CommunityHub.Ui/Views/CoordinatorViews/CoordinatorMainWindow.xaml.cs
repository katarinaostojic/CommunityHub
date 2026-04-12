using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CoordinatorMainWindow : Window
{
    public static CoordinatorMainWindow Instance { get; private set; }
    private readonly long _userId;

    public CoordinatorMainWindow(long userId)
    {
        InitializeComponent();
        Instance = this;
        _userId = userId;
    }

    public void NavigateTo(Page page, string title)
    {
        PageTitleTextBlock.Text = title;
        DashboardGrid.Visibility = Visibility.Collapsed;
        MainFrame.Visibility = Visibility.Visible;
        MainFrame.Navigate(page);
    }

    public void NavigateToDashboard()
    {
        PageTitleTextBlock.Text = "KvartApp";
        DashboardGrid.Visibility = Visibility.Visible;
        MainFrame.Visibility = Visibility.Collapsed;
        MainFrame.Content = null;
    }

    private void HamburgerButton_Click(object sender, MouseButtonEventArgs e)
    {
        NavigateToDashboard();
    }

    private void MyDistrictsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo(new MyDistrictsPage(_userId), "My Districts");
    }

    private void RequestsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo(new ManageRequestsPage(_userId), "Requests");
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        this.Close();
    }
}