using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class ConfirmationDialog : Window
{
    public ConfirmationDialog(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }

    private void OkayButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}