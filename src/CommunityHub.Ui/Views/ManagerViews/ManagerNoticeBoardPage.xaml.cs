using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerNoticeBoardPage : Page
{
    private readonly User _currentUser;
    private readonly ManagerNoticeBoardViewModel _viewModel;

    public ManagerNoticeBoardPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new ManagerNoticeBoardViewModel(_currentUser.Id);
        DataContext = _viewModel;
    }

    private void ViewNoticeboard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BuildingDto building)
        {
            NavigationService.Navigate(
                new ManagerNoticeBoardDetailsPage(_currentUser, building.Id, building.FullAddress));
        }
    }
}