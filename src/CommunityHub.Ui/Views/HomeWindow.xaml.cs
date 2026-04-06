using System.Windows;

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
    }

    private void RequestsButton_Click(object sender, RoutedEventArgs e)
    {
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        this.Close();
    }
}