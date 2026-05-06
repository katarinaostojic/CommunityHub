using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;

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
        _requestService = new NeighborhoodAccessRequestService();
        LoadRequests();
    }

    private void LoadRequests()
    {
        string? statusFilter = _currentFilter == null ? null : StatusToString(_currentFilter.Value);

        var requests = _requestService.GetAllByCoordinator(_coordinatorId, statusFilter, _sortDescending)
            .Select(r => new NeighborhoodAccessRequestDisplay(r))
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
        var display = (NeighborhoodAccessRequestDisplay)((Button)sender).Tag;

        try
        {
            _requestService.ApproveRequestWithMembership(display.Request);
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
        var display = (NeighborhoodAccessRequestDisplay)((Button)sender).Tag;

        try
        {
            RejectReasonWindow rejectWindow = new RejectReasonWindow();
            rejectWindow.ShowDialog();

            if (rejectWindow.Confirmed)
            {
                _requestService.RejectRequestForCoordinator(display.Request, rejectWindow.Reason);
                MessageBox.Show("Request rejected.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadRequests();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private class NeighborhoodAccessRequestDisplay
    {
        private readonly NeighborhoodAccessRequest _request;

        public NeighborhoodAccessRequestDisplay(NeighborhoodAccessRequest request)
        {
            _request = request;
        }

        public long Id => _request.Id;
        public User Citizen => _request.Citizen;
        public Neighborhood Neighborhood => _request.Neighborhood;
        public DateTime CreatedAt => _request.CreatedAt;
        public RequestStatus Status => _request.Status;

        public NeighborhoodAccessRequest Request => _request;

        public string StatusDisplay => _request.Status switch
        {
            RequestStatus.PendingApproval => "⏳ Pending approval",
            RequestStatus.Approved => "✔ Approved",
            RequestStatus.Rejected => "✕ Rejected",
            _ => _request.Status.ToString()
        };

        public string RejectionReasonDisplay => _request.RejectionReason != null
            ? $"Note: {_request.RejectionReason}"
            : string.Empty;

        public bool ApproveRejectVisible => _request.Status == RequestStatus.PendingApproval;
        public bool RejectionReasonVisible => _request.Status == RequestStatus.Rejected
                                           && _request.RejectionReason != null;
    }
}