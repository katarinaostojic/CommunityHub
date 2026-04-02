using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace CommunityHub.Ui.Views.TenantViews;

using CommunityHub.Ui.ViewModels;

public partial class MyRequestsPage : Page
{
    private readonly BuildingAccessRequestDbRepository _requestRepository;
    private readonly User _user;
    private List<BuildingAccessRequestViewModel> _allRequests;
    private List<BuildingAccessRequestViewModel> _filteredRequests;
    private string _currentFilter = "all";
    private bool _sortDescending = true;

    public MyRequestsPage(User user)
    {
        InitializeComponent();
        _requestRepository = new BuildingAccessRequestDbRepository();
        _user = user;
        UserNameTextBlock.Text = char.ToUpper(_user.Name[0]) + _user.Name.Substring(1).ToLower();
        LoadRequests();
        UpdateFilterButtons();
    }

    private void LoadRequests()
    {
        var requests = _requestRepository.GetAllByTenant(_user.Id);
        _allRequests = requests.Select(r => new BuildingAccessRequestViewModel
        {
            Id = r.Id,
            Building = r.Building,
            UnitNumber = r.UnitNumber,
            CreatedAt = r.CreatedAt,
            Status = r.Status,
            RejectionReason = r.RejectionReason
        }).ToList();

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
        int all = _allRequests.Count;
        int pending = _allRequests.Count(r => r.Status == "pending approval");
        int accepted = _allRequests.Count(r => r.Status == "accepted");
        int rejected = _allRequests.Count(r => r.Status == "rejected");

        FilterAllButton.Content = $"All ({all})";
        FilterPendingButton.Content = $"Pending approval ({pending})";
        FilterAcceptedButton.Content = $"Accepted ({accepted})";
        FilterRejectedButton.Content = $"Rejected ({rejected})";
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
        BuildingAccessRequestViewModel request = (BuildingAccessRequestViewModel)((Button)sender).Tag;

        CancelConfirmDialog confirmDialog = new CancelConfirmDialog(request.Building.Street, request.Building.StreetNumber);
        confirmDialog.Owner = Window.GetWindow(this);
        bool? result = confirmDialog.ShowDialog();

        if (result == true)
        {
            _requestRepository.Delete(request.Id);
            LoadRequests();
            UpdateFilterButtons();

            SuccessTextBlock.Text = "✔ Request cancelled successfully.";
            SuccessBanner.Visibility = Visibility.Visible;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, args) =>
            {
                SuccessBanner.Visibility = Visibility.Collapsed;
                timer.Stop();
            };
            timer.Start();
        }
    }
}