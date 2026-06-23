using System;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class CitizenNavBar : UserControl
{
    // Eventi koje parent stranica sluša
    public event Action? BurgerClicked;
    public event Action? BackNavigated;
    public event Action? ForwardNavigated;
    public event Action? ReloadRequested;
    public event Action? ProfileClicked;

    public CitizenNavBar()
    {
        InitializeComponent();
        Loaded += (_, _) => SyncToggles();
    }

    // Setter za naslov i URL iz parent stranice
    public void SetTitle(string title) => PageTitleTextBlock.Text = title;
    public void SetUrl(string url) => UrlTextBlock.Text = url;
    public void SetUsername(string username) => LoggedInUserTextBlock.Text = username;
    public void UpdateNavButtons()
    {
        BackButton.IsEnabled = NavigationHistory.CanGoBack;
        ForwardButton.IsEnabled = NavigationHistory.CanGoForward;
    }

    private void SyncToggles()
    {
        ThemeToggleButton.IsChecked = ThemeManager.IsDark;
        LanguageToggleButton.IsChecked = !LanguageManager.IsSerbianActive;
    }

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
        => BurgerClicked?.Invoke();

    private void BackButton_Click(object sender, RoutedEventArgs e)
        => BackNavigated?.Invoke();

    private void ForwardButton_Click(object sender, RoutedEventArgs e)
        => ForwardNavigated?.Invoke();

    private void ReloadButton_Click(object sender, RoutedEventArgs e)
        => ReloadRequested?.Invoke();

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
        => ProfileClicked?.Invoke();

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
        => ThemeManager.ToggleTheme();

    private void LanguageToggle_Click(object sender, RoutedEventArgs e)
        => LanguageManager.ToggleLanguage();
}
