using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class RejectConfirmationDialog : Window
{
    public bool WantsExplanation { get; private set; }

    public RejectConfirmationDialog()
    {
        InitializeComponent();
    }

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        WantsExplanation = true;
        DialogResult = true;
        Close();
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        WantsExplanation = false;
        DialogResult = true;
        Close();
    }
}