using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ManageRequestsPage : Page
{
    private readonly ManageRequestsViewModel _viewModel;

    public ManageRequestsPage(long coordinatorId, string? neighborhoodName = null)
    {
        InitializeComponent();
        NeighborhoodAccessRequestService requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        _viewModel = new ManageRequestsViewModel(requestService, coordinatorId, neighborhoodName);
        DataContext = _viewModel;
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();
    private void FilterPendingButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterPending();
    private void FilterApprovedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterApproved();
    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterRejected();
    private void SortButton_Click(object sender, RoutedEventArgs e) => _viewModel.ToggleSort();

    private void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;
        vm.OpenApproveConfirm();
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;
        vm.OpenDeclineConfirm();
    }

    private void ConfirmApproveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;
        try
        {
            _viewModel.Approve(vm);
            vm.CloseConfirm();
            vm.MarkAsApproved();
            vm.SuccessMessage = "Request approved successfully!";
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorBanner.Visibility = Visibility.Visible;
        }
    }

    private void ConfirmRejectButton_Click(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)button.Tag;

        var panel = button.Parent as StackPanel;
        var border = panel?.Parent as Border;
        var outerPanel = border?.Parent as StackPanel;
        var textBox = outerPanel?.Children.OfType<TextBox>().FirstOrDefault(t => t.Name == "RejectReasonBox");
        string? reason = textBox?.Text.Trim();
        if (string.IsNullOrWhiteSpace(reason)) reason = null;

        try
        {
            _viewModel.Reject(vm, reason);
            vm.CloseConfirm();
            vm.MarkAsRejected();

            vm.SuccessMessage = "Request rejected.";
        }
        catch (Exception ex)
        {
            ErrorText.Text = ex.Message;
            ErrorBanner.Visibility = Visibility.Visible;
        }
    }

    private void CancelConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;
        vm.CloseConfirm();
    }
}