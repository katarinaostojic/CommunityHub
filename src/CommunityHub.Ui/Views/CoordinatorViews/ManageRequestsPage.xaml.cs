using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ManageRequestsPage : Page
{
    private readonly ManageRequestsViewModel _viewModel;

    public ManageRequestsPage(long coordinatorId)
    {
        InitializeComponent();
        NeighborhoodAccessRequestService requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        _viewModel = new ManageRequestsViewModel(requestService, coordinatorId);
        DataContext = _viewModel;
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();
    private void FilterPendingButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterPending();
    private void FilterApprovedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterApproved();
    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterRejected();

    private void SortButton_Click(object sender, RoutedEventArgs e) => _viewModel.ToggleSort();

    private void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        var requestViewModel = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;
        try
        {
            _viewModel.Approve(requestViewModel);
            MessageBox.Show("Request approved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        var requestViewModel = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;
        try
        {
            RejectReasonWindow rejectWindow = new RejectReasonWindow();
            rejectWindow.ShowDialog();
            if (rejectWindow.Confirmed)
            {
                _viewModel.Reject(requestViewModel, rejectWindow.Reason);
                MessageBox.Show("Request rejected.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}