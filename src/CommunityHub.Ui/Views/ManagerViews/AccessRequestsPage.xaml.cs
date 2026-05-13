using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
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
        _requestService = Injector.CreateInstance<BuildingAccessRequestService>();
        LoadRequests();
    }

    private void LoadRequests()
    {
        List<BuildingAccessRequest> requests = _requestService.GetAllByManager(
            _currentUser.Id, _currentStatusFilter, _sortDescending);

        List<BuildingAccessRequestViewModel> viewModels = requests
            .Select(r => new BuildingAccessRequestViewModel(r))
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
        if (sender is Button btn && btn.Tag is BuildingAccessRequestViewModel vm)
        {
            _requestService.ApproveRequest(vm.Request);
            LoadRequests();

            ShowConfirmationDialog("A request has been accepted successfully.");
        }
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        if (!(sender is Button btn && btn.Tag is BuildingAccessRequestViewModel vm)) return;

        string? explanation = AskForRejectionExplanation();
        if (explanation == null) return;

        _requestService.RejectRequest(vm.Request, string.IsNullOrEmpty(explanation) ? null : explanation);
        LoadRequests();
        ShowConfirmationDialog("A request has been rejected successfully.");
    }

    private string? AskForRejectionExplanation()
    {
        var askDialog = new RejectConfirmationDialog();
        askDialog.Owner = Window.GetWindow(this);
        if (askDialog.ShowDialog() != true) return null;

        if (!askDialog.WantsExplanation) return string.Empty;

        var explDialog = new WriteExplanationDialog();
        explDialog.Owner = Window.GetWindow(this);
        if (explDialog.ShowDialog() != true) return null;

        return explDialog.ExplanationText;
    }

    private void ShowConfirmationDialog(string message)
    {
        var dialog = new ConfirmationDialog(message);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
    }

    private void ExplanationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BuildingAccessRequestViewModel vm)
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

    
}