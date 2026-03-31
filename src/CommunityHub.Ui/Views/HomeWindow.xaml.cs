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

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        ProfileWindow profileWindow = new ProfileWindow(_userId);
        profileWindow.Show();
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
        this.Hide();
    }

    private void LogoutButton_Click(object sender, RoutedEventArgs e)
    {
        LogInForm loginForm = new LogInForm();
        loginForm.Show();
        this.Close();
    }
    private void RegisterNeighborhoodButton_Click(object sender, RoutedEventArgs e)
    {
        RegisterNeighborhoodWindow window = new RegisterNeighborhoodWindow(_userId);
        window.Show();
        this.Hide();
    }
}
