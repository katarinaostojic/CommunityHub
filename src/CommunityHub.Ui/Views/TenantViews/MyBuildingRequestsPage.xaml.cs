using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityHub.Application.Services.TenantServices;
using CommunityHub.Ui.Helpers;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class MyBuildingRequestsPage : Page
{
    private readonly User _user;
    private List<BuildingAccessRequestDisplay> _allRequests;
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
        _allRequests = _requestService.GetAllByTenant(_user.Id)
        .Select(r => new BuildingAccessRequestDisplay(r))
        .ToList();
        ApplyFilterAndSort();
    }

    private void ApplyFilterAndSort()
    {
        _filteredRequests = _currentFilter == null
            ? _allRequests.ToList()
            : _allRequests.Where(r => r.Status == _currentFilter).ToList();

        _filteredRequests = _sortDescending
            ? _filteredRequests.OrderByDescending(r => r.CreatedAt).ToList()
            : _filteredRequests.OrderBy(r => r.CreatedAt).ToList();

        RequestsPanel.ItemsSource = _filteredRequests;
        ResultsCountText.Text = $"Showing {_filteredRequests.Count} results";
    }

    private void UpdateFilterButtons()
    {
        UpdateFilterButton(FilterAllButton, "All", _allRequests.Count);
        UpdateFilterButton(FilterPendingButton, "Pending approval", _allRequests.Count(r => r.Status == RequestStatus.PendingApproval));
        UpdateFilterButton(FilterApprovedButton, "Approved", _allRequests.Count(r => r.Status == RequestStatus.Approved));
        UpdateFilterButton(FilterRejectedButton, "Rejected", _allRequests.Count(r => r.Status == RequestStatus.Rejected));
    }

    private void UpdateFilterButton(Button filterButton, string label, int count)
    {
        filterButton.Content = $"{label} ({count})";
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = null;
        ApplyFilterAndSort();
    }

    private void FilterPendingButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = RequestStatus.PendingApproval;
        ApplyFilterAndSort();
    }

    private void FilterApprovedButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = RequestStatus.Approved;
        ApplyFilterAndSort();
    }

    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = RequestStatus.Rejected;
        ApplyFilterAndSort();
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
    {
        _sortDescending = !_sortDescending;
        SortButton.Content = _sortDescending ? "Sort by Date ↓" : "Sort by Date ↑";
        ApplyFilterAndSort();
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