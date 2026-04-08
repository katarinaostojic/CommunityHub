using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityHub.Ui.Helpers;
using CommunityHub.Application.Services;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class MyBuildingRequestsPage : Page
{
    private readonly User _user;
    private List<BuildingAccessRequestDisplay> _filteredRequests;
    private RequestStatus? _currentFilter = null; // null = "all"
    private bool _sortDescending = true;
    private readonly BuildingAccessRequestService _requestService;

    public MyBuildingRequestsPage(User user)
    {
        InitializeComponent();
        _requestService = new BuildingAccessRequestService();
        _user = user;
        UserNameTextBlock.Text = char.ToUpper(_user.Name[0]) + _user.Name.Substring(1).ToLower();
        LoadRequests();
        UpdateFilterButtons();
        AppMenu.Initialize(_user);
    }

    private void LoadRequests()
    {
        string? statusFilter = _currentFilter == null ? null : StatusToString(_currentFilter.Value);
        _filteredRequests = _requestService.GetAllByTenant(_user.Id, statusFilter, _sortDescending)
            .Select(r => new BuildingAccessRequestDisplay(r))
            .ToList();

        RequestsPanel.ItemsSource = _filteredRequests;
        ResultsCountText.Text = $"Showing {_filteredRequests.Count} results";
    }

    private string StatusToString(RequestStatus status) => status switch
    {
        RequestStatus.PendingApproval => "pending approval",
        RequestStatus.Approved => "accepted",
        RequestStatus.Rejected => "rejected",
        _ => throw new ArgumentException($"Unknown status: {status}")
    };

    private void UpdateFilterButtons()
    {
        UpdateFilterButton(FilterAllButton, "All", _requestService.CountByTenantAndStatus(_user.Id, null));
        UpdateFilterButton(FilterPendingButton, "Pending approval", _requestService.CountByTenantAndStatus(_user.Id, "pending approval"));
        UpdateFilterButton(FilterApprovedButton, "Approved", _requestService.CountByTenantAndStatus(_user.Id, "accepted"));
        UpdateFilterButton(FilterRejectedButton, "Rejected", _requestService.CountByTenantAndStatus(_user.Id, "rejected"));
    }

    private void UpdateFilterButton(Button filterButton, string label, int count)
    {
        filterButton.Content = $"{label} ({count})";
    }

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

    private void CancelRequestButton_Click(object sender, RoutedEventArgs e)
    {
        var display = (BuildingAccessRequestDisplay)((Button)sender).Tag;
        if (!ConfirmCancellation(display.Building.Street, display.Building.StreetNumber)) return;
        _requestService.Delete(display.Id);

        LoadRequests();
        UpdateFilterButtons();
        TenantBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Request cancelled successfully.");
    }

    private bool ConfirmCancellation(string street, string streetNumber)
    {
        CancelBuildingAccessRequestDialog dialog = new CancelBuildingAccessRequestDialog(street, streetNumber);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }

    //klasa za displej
    private class BuildingAccessRequestDisplay
    {
        private readonly BuildingAccessRequest _request;

        public BuildingAccessRequestDisplay(BuildingAccessRequest request)
        {
            _request = request;
        }

        public long Id => _request.Id;
        public Building Building => _request.Building;
        public string UnitNumber => _request.UnitNumber;
        public DateTime CreatedAt => _request.CreatedAt;
        public RequestStatus Status => _request.Status;

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

        public bool CancelButtonVisible => _request.Status == RequestStatus.PendingApproval;

        public bool RejectionReasonVisible => _request.Status == RequestStatus.Rejected
                                           && _request.RejectionReason != null;
    }
}