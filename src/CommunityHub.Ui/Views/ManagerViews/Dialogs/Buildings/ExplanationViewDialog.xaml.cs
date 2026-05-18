using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class ExplanationViewDialog : Window
{
    public ExplanationViewDialog(string? explanation)
    {
        InitializeComponent();
        ExplanationText.Text = string.IsNullOrWhiteSpace(explanation)
            ? "No explanation was provided."
            : explanation;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}