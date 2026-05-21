using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class MyBuildingRequestsPage : Page
{
    private readonly User _user;
    private readonly MyBuildingRequestsViewModel _viewModel;

    public MyBuildingRequestsPage(User user)
    {
        InitializeComponent();
        _user = user;

        BuildingAccessRequestService requestService = Injector.CreateInstance<BuildingAccessRequestService>();
        _viewModel = new MyBuildingRequestsViewModel(requestService, user.Id);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
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

    private bool ConfirmCancellation(string fullAddress)
    {
        CancelBuildingAccessRequestDialog dialog = new CancelBuildingAccessRequestDialog(fullAddress);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}