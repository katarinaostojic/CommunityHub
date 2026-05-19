using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.CommonRooms;
using System.Windows;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class SelectAlternativeDialog : Window
{
    private readonly long _requestId;
    private readonly CommonRoomRequestViewModel _viewModel;

    public SelectAlternativeDialog(List<string> alternatives, long requestId,
                                   CommonRoomRequestViewModel viewModel)
    {
        InitializeComponent();
        _requestId = requestId;
        _viewModel = viewModel;
        AlternativesListBox.ItemsSource = alternatives;
    }

    private void ProposeButton_Click(object sender, RoutedEventArgs e)
    {
        if (AlternativesListBox.SelectedIndex >= 0)
        {
            _viewModel.ProposeAlternative(_requestId, AlternativesListBox.SelectedIndex);
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Please select an alternative.", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}