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
        NavigateToDashboard();
    }
    private void BackButton_Click(object sender, MouseButtonEventArgs e)
    {
        if (MainFrame.CanGoBack)
            MainFrame.GoBack();
    }

    public void NavigateTo(Page page, string title)
    {
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
}