using System.Windows;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class ClearNotificationsDialog : Window
{
    public ClearNotificationsDialog()
    {
        InitializeComponent();
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void GoBackButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}