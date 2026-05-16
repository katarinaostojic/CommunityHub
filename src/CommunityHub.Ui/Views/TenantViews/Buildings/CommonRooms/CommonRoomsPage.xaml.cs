using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.Services.Buildings.CommonRooms;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;
using CommunityHub.Ui.Views.ManagerViews;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews.Buildings;

public partial class CommonRoomsPage : Page
{
    private readonly User _user;
    private readonly long _buildingId;
    private readonly string _buildingInfo;
    private readonly CommonRoomsViewModel _viewModel;

    private CommonRoomCardViewModel? _selectedRoom;

    public CommonRoomsPage(User user, long buildingId, string buildingInfo)
    {
        InitializeComponent();
        _user = user;
        _buildingId = buildingId;
        _buildingInfo = buildingInfo;

        CommonRoomService commonRoomService = Injector.CreateInstance<CommonRoomService>();
        CommonRoomRequestService requestService = Injector.CreateInstance<CommonRoomRequestService>();

        _viewModel = new CommonRoomsViewModel(commonRoomService, requestService, user.Id, buildingId);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        BuildingInfoTextBlock.Text = $"Building: {_buildingInfo}";
        AppMenu.Initialize(_user);
    }

    private void RequestButton_Click(object sender, RoutedEventArgs e)
    {
        _selectedRoom = (CommonRoomCardViewModel)((Button)sender).Tag;
        DialogRoomName.Text = _selectedRoom.Name;
        DialogFloor.Text = _selectedRoom.FloorDisplay;
        DialogRentalType.Text = _selectedRoom.RentalTypeDisplay;
        DateFromPicker.SelectedDate = null;
        DateToPicker.SelectedDate = null;
        RequestOverlay.Visibility = Visibility.Visible;
    }

    private void DialogCancelButton_Click(object sender, RoutedEventArgs e)
    {
        RequestOverlay.Visibility = Visibility.Collapsed;
        _selectedRoom = null;
    }

    private void DialogSendButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedRoom == null) return;

        if (DateFromPicker.SelectedDate == null || DateToPicker.SelectedDate == null)
        {
            MessageBox.Show("Please select both dates.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DateTime dateFrom = DateFromPicker.SelectedDate.Value;
        DateTime dateTo = DateToPicker.SelectedDate.Value;

        if (dateTo < dateFrom)
        {
            MessageBox.Show("End date must be after start date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _viewModel.SendRequest(_selectedRoom.Id, dateFrom, dateTo);
        RequestOverlay.Visibility = Visibility.Collapsed;
        _selectedRoom = null;

        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Common room request sent successfully!");
    }

    private void MyRequestsTab_Click(object sender, RoutedEventArgs e)
    {
        //MainWindow.Instance.NavigateTo(
        //   new MyCommonRoomRequestsPage(_user, _buildingId, _buildingInfo));
    }

    private void MyRequestsLink_Click(object sender, RoutedEventArgs e)
    {
        //MainWindow.Instance.NavigateTo(
        //    new MyCommonRoomRequestsPage(_user, _buildingId, _buildingInfo));
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        //MainWindow.Instance.NavigateTo(
        //    new MyCommonRoomRequestsPage(_user, _buildingId, _buildingInfo));
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}