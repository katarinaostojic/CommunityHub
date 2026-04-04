using System.Windows;
using CommunityHub.Ui.Views.CoordinatorViews;

namespace CommunityHub.Ui.Views;

public partial class HomeWindow : Window
{
    private readonly long _userId;

    public HomeWindow(long userId)
    {
        InitializeComponent();
        _userId = userId;
    }

    private void MyDistrictsButton_Click(object sender, RoutedEventArgs e)
    {
        RegisterNeighborhoodWindow window = new RegisterNeighborhoodWindow(_userId);
        window.Show();
        this.Hide();
    }

    private void RequestsButton_Click(object sender, RoutedEventArgs e)
    {
        ManageRequestsWindow window = new ManageRequestsWindow(_userId);
        window.Show();
        this.Hide();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        this.Close();
    }
}