using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.ResidentMeetings;
using System.Windows;

namespace CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;

public partial class SuggestResidentMeetingTopicDialog : Window
{
    private readonly SuggestResidentMeetingTopicDialogViewModel _viewModel;

    public SuggestResidentMeetingTopicDialog(SuggestResidentMeetingTopicDialogViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    public string Topic => _viewModel.Topic.Trim();

    private void SuggestTopicButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.Validate())
            return;

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}