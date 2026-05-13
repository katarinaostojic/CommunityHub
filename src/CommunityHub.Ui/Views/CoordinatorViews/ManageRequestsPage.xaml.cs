using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ManageRequestsPage : Page
{
    private readonly long _coordinatorId;
    private readonly NeighborhoodAccessRequestService _requestService;
    private RequestStatus? _currentFilter = null;
    private bool _sortDescending = true;

    public ManageRequestsPage(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        LoadRequests();
    }

    private void LoadRequests()
    {
        string? statusFilter = _currentFilter == null ? null : StatusToString(_currentFilter.Value);

        var requests = _requestService.GetAllByCoordinator(_coordinatorId, statusFilter, _sortDescending)
            .Select(r => new NeighborhoodAccessRequestCoordinatorViewModel(r))
            .ToList();

        RequestsItemsControl.ItemsSource = requests;
    }

    private string StatusToString(RequestStatus status) => status switch
    {
        RequestStatus.PendingApproval => "pending approval",
        RequestStatus.Approved => "accepted",
        RequestStatus.Rejected => "rejected",
        _ => throw new ArgumentException($"Unknown status: {status}")
    };

    private void FilterAllButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = null;
        LoadRequests();
    }

    private void FilterPendingButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = RequestStatus.PendingApproval;
        LoadRequests();
    }

    private void FilterApprovedButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = RequestStatus.Approved;
        LoadRequests();
    }

    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = RequestStatus.Rejected;
        LoadRequests();
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        _sortDescending = !_sortDescending;
        SortButton.Content = _sortDescending ? "Sort by Date ↓" : "Sort by Date ↑";
        LoadRequests();
    }

    private void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;

        try
        {
            _requestService.ApproveRequestWithMembership(vm.Request);
            MessageBox.Show("Request approved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadRequests();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        var vm = (NeighborhoodAccessRequestCoordinatorViewModel)((Button)sender).Tag;

        try
        {
            RejectReasonWindow rejectWindow = new RejectReasonWindow();
            rejectWindow.ShowDialog();

            if (rejectWindow.Confirmed)
            {
                _requestService.RejectRequestForCoordinator(vm.Request, rejectWindow.Reason);
                MessageBox.Show("Request rejected.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRequests();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}