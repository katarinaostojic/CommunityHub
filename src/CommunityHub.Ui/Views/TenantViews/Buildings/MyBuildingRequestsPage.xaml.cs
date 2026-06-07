using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using CommunityHub.Ui.Views.TenantViews.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class MyBuildingRequestsPage : Page
{
    private readonly User _user;
    private readonly MyBuildingRequestsViewModel _viewModel;
    private readonly BuildingMembershipService _membershipService;

    public MyBuildingRequestsPage(User user)
    {
        InitializeComponent();
        _user = user;

        BuildingAccessRequestService requestService = Injector.CreateInstance<BuildingAccessRequestService>();
        _membershipService = Injector.CreateInstance<BuildingMembershipService>();

        _viewModel = new MyBuildingRequestsViewModel(requestService, user.Id);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        NotificationBell.Initialize(_user);
        AppMenu.Initialize(_user);
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();

    private void FilterPendingButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterPending();

    private void FilterApprovedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterApproved();

    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterRejected();

    private void SortButton_Click(object sender, RoutedEventArgs e) => _viewModel.ToggleSort();

    private void CancelRequestButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingAccessRequestViewModel item = (BuildingAccessRequestViewModel)((Button)sender).Tag;
        if (!ConfirmCancellation(item.BuildingAddress)) return;

        _viewModel.CancelRequest(item.Id);
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Request cancelled successfully.");
    }

    private void BrowseOtherBuildingsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new BrowseBuildingsPage(_user));
    }

    private void NoticeBoardButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingAccessRequestViewModel request = GetSelectedRequest(sender);
        BuildingMembershipDto? membership = GetMembership(request.BuildingId);

        if (membership == null)
            return;

        NavigationService.Navigate(new NoticeBoardPage(_user, membership));
    }

    private void CommonRoomsButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingAccessRequestViewModel request = GetSelectedRequest(sender);
        BuildingMembershipDto? membership = GetMembership(request.BuildingId);

        if (membership == null)
            return;

        string buildingInfo = $"{membership.BuildingStreet} {membership.BuildingStreetNumber}, {membership.BuildingNeighborhood}";
        NavigationService.Navigate(new CommonRoomsPage(_user, membership.BuildingId, buildingInfo));
    }

    private void ResidentsMeetingButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingAccessRequestViewModel request = GetSelectedRequest(sender);
        BuildingMembershipDto? membership = GetMembership(request.BuildingId);

        if (membership == null)
            return;

        NavigationService.Navigate(new ResidentsMeetingsPage(_user, membership));
    }

    private void ReportProblemButton_Click(object sender, RoutedEventArgs e)
    {
        BuildingAccessRequestViewModel request = GetSelectedRequest(sender);
        BuildingMembershipDto? membership = GetMembership(request.BuildingId);

        if (membership == null)
            return;

        NavigationService.Navigate(new ReportedProblemsPage(_user, membership));
    }

    private BuildingAccessRequestViewModel GetSelectedRequest(object sender)
    {
        return (BuildingAccessRequestViewModel)((Button)sender).Tag;
    }

    private BuildingMembershipDto? GetMembership(long buildingId)
    {
        return _membershipService
            .GetByTenant(_user.Id)
            .FirstOrDefault(membership => membership.BuildingId == buildingId);
    }
    private bool ConfirmCancellation(string fullAddress)
    {
        CancelBuildingAccessRequestDialog dialog = new CancelBuildingAccessRequestDialog(fullAddress);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}