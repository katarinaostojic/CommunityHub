using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class BuildingDetailsPage : Page
{
    private readonly User _currentUser;
    private readonly BuildingDetailsViewModel _viewModel;

    public BuildingDetailsPage(User user, long buildingId)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new BuildingDetailsViewModel(buildingId);
        DataContext = _viewModel;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void TabBuildingInfo_Click(object sender, RoutedEventArgs e)
    {
        PanelBuildingInfo.Visibility = Visibility.Visible;
        PanelCommonRoom.Visibility = Visibility.Collapsed;
        SetActiveTab(TabBuildingInfo);
        SetInactiveTab(TabCommonRoom);
    }

    private void TabCommonRoom_Click(object sender, RoutedEventArgs e)
    {
        PanelBuildingInfo.Visibility = Visibility.Collapsed;
        PanelCommonRoom.Visibility = Visibility.Visible;
        SetActiveTab(TabCommonRoom);
        SetInactiveTab(TabBuildingInfo);
        _viewModel.LoadCommonRooms();
    }

    private void AddCommonRoom_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.Building == null) return;
        AddCommonRoomDialog dialog = new AddCommonRoomDialog(
            _currentUser, _viewModel.Building, _viewModel);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
    }

    private void ViewRequests_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long commonRoomId)
        {
            CommonRoomDto? room = _viewModel.CommonRooms
                .FirstOrDefault(r => r.Id == commonRoomId);
            if (room == null) return;
            NavigationService.Navigate(
                new CommonRoomRequestsPage(_currentUser, commonRoomId, room.Name));
        }
    }

    private void SetActiveTab(Button tab)
    {
        tab.Foreground = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        tab.BorderThickness = new Thickness(0, 0, 0, 3);
        tab.BorderBrush = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        tab.FontWeight = FontWeights.SemiBold;
    }

    private void SetInactiveTab(Button tab)
    {
        tab.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
        tab.BorderThickness = new Thickness(0);
        tab.FontWeight = FontWeights.Normal;
    }

    private void ViewCalendar_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long commonRoomId)
        {
            CommonRoomDto? room = _viewModel.CommonRooms
                .FirstOrDefault(r => r.Id == commonRoomId);
            if (room == null) return;
            var dialog = new CommonRoomCalendarDialog(commonRoomId, room.Name);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }
}