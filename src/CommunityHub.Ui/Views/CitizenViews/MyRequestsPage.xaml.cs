using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.CitizenViews
{
    public partial class MyRequestsPage : Window
    {
        private readonly User _user;
        private readonly NeighborhoodAccessRequestService _service;

        private List<NeighborhoodAccessRequest> _allRequests = new();
        private List<RequestCardViewModel> _shownRequests = new();
        private bool _sortDescending = true;

        public MyRequestsPage(User user)
        {
            InitializeComponent();

            _user = user;
            _service = new NeighborhoodAccessRequestService();

            LoggedInUserTextBlock.Text = _user.Username;

            CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
            CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
            CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

            LoadRequests();
        }

        private void LoadRequests()
        {
            _allRequests = _service.GetAllByCitizen(_user.Id, null, _sortDescending);
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<NeighborhoodAccessRequest> filtered = FilterByStatus(_allRequests);
            filtered = SortByDate(filtered);
            _shownRequests = filtered.Select(BuildRequestCard).ToList();
            RefreshDisplay();
        }

        private IEnumerable<NeighborhoodAccessRequest> FilterByStatus(IEnumerable<NeighborhoodAccessRequest> requests)
        {
            string selectedStatus = GetSelectedStatusText();
            if (selectedStatus == "All")
                return requests;
            return requests.Where(r => GetStatusText(r.Status) == selectedStatus);
        }

        private IEnumerable<NeighborhoodAccessRequest> SortByDate(IEnumerable<NeighborhoodAccessRequest> requests)
        {
            return _sortDescending
                ? requests.OrderByDescending(r => r.CreatedAt)
                : requests.OrderBy(r => r.CreatedAt);
        }

        private RequestCardViewModel BuildRequestCard(NeighborhoodAccessRequest r)
        {
            string imagePath = GetNeighborhoodImagePath(r);
            return new RequestCardViewModel
            {
                Id = r.Id,
                NeighborhoodName = r.Neighborhood?.Name ?? string.Empty,
                CreatedAtFormatted = r.CreatedAt.ToString("dd/MM/yyyy"),
                StatusText = GetStatusText(r.Status),
                StatusBrush = GetStatusBrush(r.Status),
                DeleteVisibility = IsDeleteVisible(r.Status),
                RejectionVisibility = IsRejectionVisible(r.RejectionReason),
                RejectionReason = r.RejectionReason ?? string.Empty,
                ImagePath = imagePath,
                NoImageVisibility = string.IsNullOrWhiteSpace(imagePath)
                    ? Visibility.Visible
                    : Visibility.Collapsed
            };
        }

        private Visibility IsDeleteVisible(RequestStatus status)
        {
            return status == RequestStatus.PendingApproval ? Visibility.Visible : Visibility.Collapsed;
        }

        private Visibility IsRejectionVisible(string? rejectionReason)
        {
            return string.IsNullOrWhiteSpace(rejectionReason) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void RefreshDisplay()
        {
            RequestsItemsControl.ItemsSource = null;
            RequestsItemsControl.ItemsSource = _shownRequests;
            ResultsTextBlock.Text = $"Showing {_shownRequests.Count} requests";
        }

        private string GetNeighborhoodImagePath(NeighborhoodAccessRequest request)
        {
            if (request?.Neighborhood?.Images == null || request.Neighborhood.Images.Count == 0)
                return string.Empty;

            var firstImage = request.Neighborhood.Images.FirstOrDefault();
            return firstImage == null || string.IsNullOrWhiteSpace(firstImage.Path)
                ? string.Empty
                : firstImage.Path;
        }

        private string GetSelectedStatusText()
        {
            ComboBoxItem selectedItem = (ComboBoxItem)StatusFilterComboBox.SelectedItem;
            return selectedItem.Content?.ToString() ?? "All";
        }

        private string GetStatusText(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.PendingApproval => "Pending approval",
                RequestStatus.Approved => "Approved",
                RequestStatus.Rejected => "Rejected",
                _ => "Unknown"
            };
        }

        private Brush GetStatusBrush(RequestStatus status)
        {
            return status switch
            {
                RequestStatus.PendingApproval => Brushes.Gold,
                RequestStatus.Approved => Brushes.LimeGreen,
                RequestStatus.Rejected => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        private void Filters_Changed(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            ApplyFilters();
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            _sortDescending = !_sortDescending;
            SortButton.Content = _sortDescending ? "Date ↓" : "Date ↑";
            ApplyFilters();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            StatusFilterComboBox.SelectedIndex = 0;
            _sortDescending = true;
            SortButton.Content = "Date ↓";
            ApplyFilters();
        }

        private void DeleteRequestButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            long requestId = Convert.ToInt64(button.Tag);

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to delete this request?",
                "Delete request",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _service.Delete(requestId);
            LoadRequests();
        }

        private void BurgerButton_Click(object sender, RoutedEventArgs e)
        {
            CitizenMenu.Visibility = Visibility.Visible;
        }

        private void CitizenMenu_CloseRequested()
        {
            CitizenMenu.Visibility = Visibility.Collapsed;
        }

        private void CitizenMenu_NavigationRequested(string destination)
        {
            CitizenMenu.Visibility = Visibility.Collapsed;

            switch (destination)
            {
                case "Neighborhoods":
                    new BrowseNeighborhoodPage(_user).Show();
                    Close();
                    break;

                case "MyRequests":
                    break;

                case "Events":
                    MessageBox.Show("Go to Events page.");
                    break;

                case "Citizens":
                    MessageBox.Show("Go to Citizens page.");
                    break;

                case "Meetings":
                    MessageBox.Show("Go to Meetings page.");
                    break;

                case "CityObjects":
                    MessageBox.Show("Go to City Objects page.");
                    break;

                case "Budget":
                    MessageBox.Show("Go to Budget page.");
                    break;
            }
        }

        private void CitizenMenu_LogoutRequested()
        {
            CitizenMenu.Visibility = Visibility.Collapsed;

            LogInForm loginForm = new LogInForm();
            loginForm.Show();
            Close();
        }
    }

    public class RequestCardViewModel
    {
        public long Id { get; set; }
        public string NeighborhoodName { get; set; } = string.Empty;
        public string CreatedAtFormatted { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public Brush StatusBrush { get; set; } = Brushes.Gray;
        public Visibility DeleteVisibility { get; set; } = Visibility.Collapsed;
        public Visibility RejectionVisibility { get; set; } = Visibility.Collapsed;
        public string RejectionReason { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public Visibility NoImageVisibility { get; set; } = Visibility.Visible;
    }
}