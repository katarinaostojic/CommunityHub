using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Interfaces.Buildings.CommonRooms;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.CommonRooms;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews.Buildings;

public partial class MyCommonRoomRequestsPage : Page
{
    private readonly User _user;
    private readonly long _buildingId;
    private readonly string _buildingInfo;
    private readonly MyCommonRoomRequestsViewModel _viewModel;

    public MyCommonRoomRequestsPage(User user, long buildingId, string buildingInfo)
    {
        InitializeComponent();
        _user = user;
        _buildingId = buildingId;
        _buildingInfo = buildingInfo;

        ICommonRoomRequestService requestService = Injector.CreateInstance<ICommonRoomRequestService>();
        _viewModel = new MyCommonRoomRequestsViewModel(requestService, user.Id, buildingId);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        BuildingInfoTextBlock.Text = $"Building: {_buildingInfo}";
        AppMenu.Initialize(_user);
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();
    private void FilterPendingButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterPending();
    private void FilterDateChangeButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterDateChange();
    private void FilterApprovedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterApproved();
    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterRejected();

    private void AcceptSuggestionButton_Click(object sender, RoutedEventArgs e)
    {
        CommonRoomRequestRowViewModel item = (CommonRoomRequestRowViewModel)((Button)sender).Tag;
        _viewModel.AcceptDateChange(item.Id);
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Date change accepted.");
    }

    private void CancelRequestButton_Click(object sender, RoutedEventArgs e)
    {
        CommonRoomRequestRowViewModel item = (CommonRoomRequestRowViewModel)((Button)sender).Tag;
        _viewModel.CancelRequest(item.Id);
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Request cancelled successfully.");
    }

    private void CommonRoomsTab_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.Instance.NavigateTo(
            new CommonRoomsPage(_user, _buildingId, _buildingInfo));
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}