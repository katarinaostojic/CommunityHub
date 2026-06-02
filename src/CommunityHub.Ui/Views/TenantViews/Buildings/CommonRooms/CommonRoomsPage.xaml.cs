using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Interfaces.Buildings.CommonRooms;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.CommonRooms;
using CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews.Buildings;

public partial class CommonRoomsPage : Page
{
    private readonly User _user;
    private readonly long _buildingId;
    private readonly string _buildingInfo;
    private readonly CommonRoomsViewModel _viewModel;

    public CommonRoomsPage(User user, long buildingId, string buildingInfo)
    {
        InitializeComponent();
        _user = user;
        _buildingId = buildingId;
        _buildingInfo = buildingInfo;

        ICommonRoomService commonRoomService = Injector.CreateInstance<ICommonRoomService>();
        ICommonRoomRequestService requestService = Injector.CreateInstance<ICommonRoomRequestService>();

        _viewModel = new CommonRoomsViewModel(commonRoomService, requestService, user.Id, buildingId);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        BuildingInfoTextBlock.Text = $"Building: {_buildingInfo}";
        AppMenu.Initialize(_user);
    }

    private void RequestButton_Click(object sender, RoutedEventArgs e)
    {
        CommonRoomCardViewModel room = (CommonRoomCardViewModel)((Button)sender).Tag;

        CommonRoomRequestDialogViewModel dialogViewModel = new CommonRoomRequestDialogViewModel(
            _buildingInfo,
            room.Name,
            room.FloorDisplay,
            room.RentalType);

        CommonRoomRequestDialog dialog = new CommonRoomRequestDialog(dialogViewModel);
        dialog.Owner = Window.GetWindow(this);

        if (dialog.ShowDialog() != true) return;

        _viewModel.SendRequest(room.Id, dialog.SelectedDateFrom!.Value, dialog.SelectedDateTo!.Value);
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Common room request sent successfully!");
    }

    private void MyRequestsTab_Click(object sender, RoutedEventArgs e) =>
        MainWindow.Instance.NavigateTo(new MyCommonRoomRequestsPage(_user, _buildingId, _buildingInfo));

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e) =>
        MainWindow.Instance.NavigateTo(new MyCommonRoomRequestsPage(_user, _buildingId, _buildingInfo));

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}