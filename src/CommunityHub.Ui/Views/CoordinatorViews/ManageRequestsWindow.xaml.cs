using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class ManageRequestsWindow : Window
{
    private readonly long _coordinatorId;
    private readonly NeighborhoodDbRepository _repository = new();

    public ManageRequestsWindow(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        LoadStatusFilter();
        LoadRequests();
    }

    private void LoadStatusFilter()
    {
        StatusFilterComboBox.Items.Add("All");
        StatusFilterComboBox.Items.Add(RequestStatus.PendingApproval.ToString());
        StatusFilterComboBox.Items.Add(RequestStatus.Approved.ToString());
        StatusFilterComboBox.Items.Add(RequestStatus.Rejected.ToString());
        StatusFilterComboBox.SelectedIndex = 0;

        SortComboBox.Items.Add("Newest First");
        SortComboBox.Items.Add("Oldest First");
        SortComboBox.SelectedIndex = 0;
    }

    private void LoadRequests()
    {
        string? filter = StatusFilterComboBox.SelectedItem?.ToString() == "All"
            ? null
            : StatusFilterComboBox.SelectedItem?.ToString();

        var requests = _repository.GetRequestsByCoordinator(_coordinatorId, filter);

        if (SortComboBox.SelectedItem?.ToString() == "Oldest First")
            requests = requests.OrderBy(r => r.CreatedAt).ToList();
        else
            requests = requests.OrderByDescending(r => r.CreatedAt).ToList();

        RequestsItemsControl.ItemsSource = requests;
    }

    private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadRequests();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadRequests();
    }

    private void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequest? selected = (sender as Button)?.Tag as NeighborhoodAccessRequest;

        if (selected == null)
        {
            MessageBox.Show("Please select a request.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (selected.Status != RequestStatus.PendingApproval)
        {
            MessageBox.Show("Only pending requests can be approved.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _repository.ApproveRequest(selected.Id, selected.Citizen.Id, selected.Neighborhood.Id);
        MessageBox.Show("Request approved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        LoadRequests();
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequest? selected = (sender as Button)?.Tag as NeighborhoodAccessRequest;

        if (selected == null)
        {
            MessageBox.Show("Please select a request.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (selected.Status != RequestStatus.PendingApproval)
        {
            MessageBox.Show("Only pending requests can be rejected.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        RejectReasonWindow rejectWindow = new RejectReasonWindow();
        rejectWindow.ShowDialog();

        if (rejectWindow.Confirmed)
        {
            _repository.RejectRequest(selected.Id, rejectWindow.Reason);
            MessageBox.Show("Request rejected.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadRequests();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}