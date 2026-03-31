using System.Windows;
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
        StatusFilterComboBox.Items.Add("ceka odobrenje");
        StatusFilterComboBox.Items.Add("prihvaćen");
        StatusFilterComboBox.Items.Add("odbijen");
        StatusFilterComboBox.SelectedIndex = 0;
    }

    private void LoadRequests()
    {
        string? filter = StatusFilterComboBox.SelectedItem?.ToString() == "All"
            ? null
            : StatusFilterComboBox.SelectedItem?.ToString();

        var requests = _repository.GetRequestsByCoordinator(_coordinatorId, filter);
        RequestsDataGrid.ItemsSource = requests;
    }

    private void StatusFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        LoadRequests();
    }

    private void ApproveButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequest? selected = RequestsDataGrid.SelectedItem as NeighborhoodAccessRequest;

        if (selected == null)
        {
            MessageBox.Show("Please select a request.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (selected.Status != "ceka odobrenje")
        {
            MessageBox.Show("Only pending requests can be approved.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _repository.ApproveRequest(selected.Id, selected.CitizenId, selected.NeighborhoodId);
        MessageBox.Show("Request approved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        LoadRequests();
    }

    private void RejectButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequest? selected = RequestsDataGrid.SelectedItem as NeighborhoodAccessRequest;

        if (selected == null)
        {
            MessageBox.Show("Please select a request.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (selected.Status != "ceka odobrenje")
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