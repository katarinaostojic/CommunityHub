using System.Windows;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class RejectReasonWindow : Window
{
    public bool Confirmed { get; private set; } = false;
    public string? Reason { get; private set; }

    public RejectReasonWindow()
    {
        InitializeComponent();
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = true;
        Reason = string.IsNullOrWhiteSpace(ReasonTextBox.Text) ? null : ReasonTextBox.Text.Trim();
        this.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Confirmed = false;
        this.Close();
    }
}