using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class WriteExplanationDialog : Window
{
    public string ExplanationText => ExplanationBox.Text.Trim();

    public WriteExplanationDialog()
    {
        InitializeComponent();
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}