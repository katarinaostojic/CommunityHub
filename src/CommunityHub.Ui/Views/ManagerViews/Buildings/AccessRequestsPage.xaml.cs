using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        SetActiveFilterButton(BtnAll);
    }

    private void FilterAll_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnAll);
        _viewModel.SetStatusFilter(null);
    }

    private void FilterPending_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnPending);
        _viewModel.SetStatusFilter("pending approval");
    }

    private void FilterApproved_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnApproved);
        _viewModel.SetStatusFilter("approved");
    }

    private void FilterRejected_Click(object sender, RoutedEventArgs e)
    {
        SetActiveFilterButton(BtnRejected);
        _viewModel.SetStatusFilter("rejected");
    }

    private void SetActiveFilterButton(Button activeButton)
    {
        var filterButtons = new[] { BtnAll, BtnPending, BtnApproved, BtnRejected };
        foreach (var btn in filterButtons)
        {
            btn.Background = new SolidColorBrush(Color.FromRgb(0xE8, 0xED, 0xF2));
            btn.Foreground = new SolidColorBrush(Color.FromRgb(0x2C, 0x3E, 0x50));
        }

        activeButton.Background = new SolidColorBrush(Color.FromRgb(0x29, 0x80, 0xB9));
        activeButton.Foreground = Brushes.White;
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