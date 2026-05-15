using CommunityHub.Application.Domain;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class CommonRoomRequestsPage : Page
{
    private readonly User _currentUser;
    private readonly CommonRoomRequestViewModel _viewModel;

    public CommonRoomRequestsPage(User user, long commonRoomId, string roomName)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new CommonRoomRequestViewModel(commonRoomId, roomName);
        DataContext = _viewModel;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void ShowFreeDays_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long requestId)
        {
            _viewModel.LoadFreeDays(requestId);
            var dialog = new SelectDayDialog(_viewModel.FreeDays.ToList(), requestId, _viewModel);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }

    private void ShowAlternatives_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long requestId)
        {
            _viewModel.LoadAlternativeRanges(requestId);
            var dialog = new SelectAlternativeDialog(
                _viewModel.AlternativeRanges.ToList(), requestId, _viewModel);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }
}