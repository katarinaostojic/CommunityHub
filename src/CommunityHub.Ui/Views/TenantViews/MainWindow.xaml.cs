using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views;

public partial class MainWindow : Window
{
    public static MainWindow Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();
        Instance = this;
    }

    public void NavigateTo(Page page)
    {
        MainFrame.Navigate(page);
    }
}