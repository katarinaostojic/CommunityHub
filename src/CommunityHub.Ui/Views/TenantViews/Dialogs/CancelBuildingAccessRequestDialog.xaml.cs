using System.Windows;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class CancelBuildingAccessRequestDialog : Window
{
    public CancelBuildingAccessRequestDialog(string street, string streetNumber)
    {
        InitializeComponent();
        MessageTextBlock.Text = $"This will permanently delete your request for {street} {streetNumber}.\nThis action cannot be undone.";
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