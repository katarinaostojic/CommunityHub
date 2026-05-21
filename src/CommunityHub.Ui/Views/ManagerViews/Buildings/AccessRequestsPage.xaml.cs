using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class AccessRequestsPage : Page
{
    private readonly User _currentUser;
    private readonly AccessRequestsViewModel _viewModel;

    public AccessRequestsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new AccessRequestsViewModel(_currentUser.Id);
        DataContext = _viewModel;
    }

    private void StatusFilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel == null) return;

        if (StatusFilterCombo.SelectedItem is ComboBoxItem item)
        {
            string tag = item.Tag?.ToString() ?? "";
            _viewModel.SetStatusFilter(string.IsNullOrEmpty(tag) ? null : tag);
        }
    }

    private void SortDateButton_Click(object sender, RoutedEventArgs e)
    {
        SortArrow.Text = _viewModel.SortDescending ? " ↑" : " ↓";
        _viewModel.ToggleSort();
    }

    private void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is long id)
        {
            _viewModel.ApproveRequest(id);
            ShowConfirmationDialog("A request has been accepted successfully.");
        }
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        if (!(sender is Button btn && btn.Tag is long id)) return;
        string? explanation = AskForRejectionExplanation();
        if (explanation == null) return;
        _viewModel.RejectRequest(id, string.IsNullOrEmpty(explanation) ? null : explanation);
        ShowConfirmationDialog("A request has been rejected successfully.");
    }

    private void ExplanationButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is AccessRequestRowViewModel vm)
        {
            var dialog = new ExplanationViewDialog(vm.RejectionReason);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
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

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (Window.GetWindow(this) is ManagerMainWindow mw)
            mw.NavigateToBuildings();
    }
}