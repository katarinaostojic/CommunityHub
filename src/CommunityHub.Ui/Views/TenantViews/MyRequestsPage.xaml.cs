using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityHub.Application.Services.TenantServices;
using CommunityHub.Ui.Helpers;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class MyRequestsPage : Page
{
    private readonly User _user;
    private List<BuildingAccessRequest> _allRequests;
    private List<BuildingAccessRequest> _filteredRequests;
    private string _currentFilter = "all";
    private bool _sortDescending = true;
    private readonly BuildingAccessRequestService _requestService;
    public MyRequestsPage(User user)
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
        _allRequests = _requestService.GetAllByTenant(_user.Id);
        ApplyFilterAndSort();
    }

    private void ApplyFilterAndSort()
    {
        _filteredRequests = _currentFilter == "all"
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
        UpdateFilterButton(FilterPendingButton, "Pending approval", _allRequests.Count(r => r.Status == "pending approval"));
        UpdateFilterButton(FilterAcceptedButton, "Accepted", _allRequests.Count(r => r.Status == "accepted"));
        UpdateFilterButton(FilterRejectedButton, "Rejected", _allRequests.Count(r => r.Status == "rejected"));
    }

    private void UpdateFilterButton(Button filterButton, string label, int count)
    {
        filterButton.Content = $"{label} ({count})";
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = "all";
        ApplyFilterAndSort();
    }

    private void FilterPendingButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = "pending approval";
        ApplyFilterAndSort();
    }

    private void FilterAcceptedButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = "accepted";
        ApplyFilterAndSort();
    }

    private void FilterRejectedButton_Click(object sender, RoutedEventArgs e)
    {
        _currentFilter = "rejected";
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
        BuildingAccessRequest request = (BuildingAccessRequest)((Button)sender).Tag;

        if (!ConfirmCancellation(request)) return;

        _requestService.Delete(request.Id);
        LoadRequests();
        UpdateFilterButtons();
        TenantBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Request cancelled successfully.");
    }

    private bool ConfirmCancellation(BuildingAccessRequest request)
    {
        CancelConfirmDialog dialog = new CancelConfirmDialog(request.Building.Street, request.Building.StreetNumber);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }
}