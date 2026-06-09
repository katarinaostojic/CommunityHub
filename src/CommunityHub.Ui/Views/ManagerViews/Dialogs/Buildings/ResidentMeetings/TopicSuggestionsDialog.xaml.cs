using CommunityHub.Ui.ViewModels.ManagerViewModels.Dialogs.Buildings.ResidentMeetings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs.Buildings.ResidentMeetings;

public partial class TopicSuggestionsDialog : Window
{
    private readonly TopicSuggestionsDialogViewModel _viewModel;

    public TopicSuggestionsDialog(TopicSuggestionsDialogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    private void AddTopic_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string topic)
            _viewModel.AddTopic(topic);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_viewModel.CanClose)
        {
            MessageBox.Show(
                "You must add at least one suggested topic before closing.",
                "Warning");
            return;
        }
        Close();
    }
}