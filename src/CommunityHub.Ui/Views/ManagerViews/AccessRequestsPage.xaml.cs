using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class AccessRequestsPage : Page
{
    private readonly User _currentUser;
    private readonly BuildingAccessRequestService _requestService;
    private bool _sortDescending = true;
    private string? _currentStatusFilter = null;

    public AccessRequestsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _requestService = new BuildingAccessRequestService();
        LoadRequests();
    }

    private void LoadRequests()
    {
        List<BuildingAccessRequest> requests = _requestService.GetAllByManager(
            _currentUser.Id, _currentStatusFilter, _sortDescending);

        List<BuildingAccessRequestDisplay> viewModels = requests
            .Select(r => new BuildingAccessRequestDisplay(r))
            .ToList();

        RequestsItemsControl.ItemsSource = viewModels;
    }

    private void StatusFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_currentUser == null) return;

        if (StatusFilterCombo.SelectedItem is ComboBoxItem item)
        {
            string tag = item.Tag?.ToString() ?? "";
            _currentStatusFilter = string.IsNullOrEmpty(tag) ? null : tag;
            LoadRequests();
        }
    }

    private void SortDateButton_Click(object sender, RoutedEventArgs e)
    {
        _sortDescending = !_sortDescending;
        SortArrow.Text = _sortDescending ? " ↓" : " ↑";
        LoadRequests();
    }

    private void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BuildingAccessRequestDisplay vm)
        {
            _requestService.ApproveRequest(vm.Request);
            LoadRequests();

            var dialog = new ConfirmationDialog("A request has been accepted successfully.");
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BuildingAccessRequestDisplay vm)
        {
            var askDialog = new RejectConfirmationDialog();
            askDialog.Owner = Window.GetWindow(this);
            bool? result = askDialog.ShowDialog();

            if (result != true) return;

            if (askDialog.WantsExplanation)
            {
                var explDialog = new WriteExplanationDialog();
                explDialog.Owner = Window.GetWindow(this);
                bool? explResult = explDialog.ShowDialog();
                if (explResult != true) return;

                _requestService.RejectRequest(vm.Request.Id, explDialog.ExplanationText);
            }
            else
            {
                _requestService.RejectRequest(vm.Request.Id, null);
            }

            LoadRequests();

            var confirmDialog = new ConfirmationDialog("A request has been rejected successfully.");
            confirmDialog.Owner = Window.GetWindow(this);
            confirmDialog.ShowDialog();
        }
    }

    private void ExplanationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BuildingAccessRequestDisplay vm)
        {
            var dialog = new ExplanationViewDialog(vm.Request.RejectionReason);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is ManagerMainWindow mw)
            mw.NavigateToBuildings();
    }

    private class BuildingAccessRequestDisplay
    {
        private readonly BuildingAccessRequest _request;

        public BuildingAccessRequestDisplay(BuildingAccessRequest request)
        {
            _request = request;
        }

        public BuildingAccessRequest Request => _request;
        public User User => _request.User;
        public Building Building => _request.Building;
        public string UnitNumber => _request.UnitNumber;
        public DateTime CreatedAt => _request.CreatedAt;
        public string? RejectionReason => _request.RejectionReason;

        public string StatusDisplay => _request.Status switch
        {
            RequestStatus.PendingApproval => "Pending",
            RequestStatus.Approved => "Accepted",
            RequestStatus.Rejected => "Rejected",
            _ => _request.Status.ToString()
        };

        public bool PendingVisible => _request.Status == RequestStatus.PendingApproval;
        public bool ExplanationVisible => _request.Status == RequestStatus.Rejected;
    }
}