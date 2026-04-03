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
        Width = 1024;
        Height = 768;
    }

    public void NavigateTo(Page page)
    {
        MainFrame.Navigate(page);
    }
}