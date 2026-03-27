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

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        ProfileWindow profileWindow = new ProfileWindow(_userId);
        profileWindow.Show();
        this.Close();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        this.Close();
    }

    private void CountriesButton_Click(object sender, RoutedEventArgs e)
    {
        CountriesWindow countriesWindow = new CountriesWindow(this);
        countriesWindow.Show();
        this.Hide();
    }

    private void CitiesButton_Click(object sender, RoutedEventArgs e)
    {
        CitiesWindow citiesWindow = new CitiesWindow();
        citiesWindow.Show();
    }

}
