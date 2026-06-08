using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

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
        MainFrame.Navigated += MainFrame_Navigated;
        NavigateToDashboard();
    }

    private void MainFrame_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.Content is Page page && !string.IsNullOrEmpty(page.Title))
            PageTitleTextBlock.Text = page.Title;
    }

    private void BackButton_Click(object sender, MouseButtonEventArgs e)
    {
        if (MainFrame.CanGoBack)
            MainFrame.GoBack();
    }

    public void NavigateTo(Page page, string title)
    {
        page.Title = title;
        PageTitleTextBlock.Text = title;
        MainFrame.Navigate(page);
    }

    public void NavigateToDashboard()
    {
        PageTitleTextBlock.Text = "KvartAPP";
        MainFrame.Navigate(new CoordinatorDashboardPage(_userId));
    }

    private void HamburgerButton_Click(object sender, MouseButtonEventArgs e)
    {
        NavigateToDashboard();
    }

    public void GoBack()
    {
        if (MainFrame.CanGoBack)
            MainFrame.GoBack();
    }
    public Page? CurrentPage => MainFrame.Content as Page;
    public event System.Windows.Navigation.NavigatedEventHandler? Navigated
    {
        add => MainFrame.Navigated += value;
        remove => MainFrame.Navigated -= value;
    }
}