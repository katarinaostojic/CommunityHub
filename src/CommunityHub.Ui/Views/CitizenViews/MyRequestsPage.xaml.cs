using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MyRequestsPage : Window
{
    private readonly User _user;
    private readonly MyRequestsViewModel _viewModel;

    public MyRequestsPage(User user)
    {
        InitializeComponent();
        _user = user;

        NeighborhoodAccessRequestService service = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        _viewModel = new MyRequestsViewModel(service, user.Id);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
    }

    private void Filters_Changed(object sender, EventArgs e)
    {
        if (!IsLoaded) return;
        string selected = ((ComboBoxItem)StatusFilterComboBox.SelectedItem).Content?.ToString() ?? "All";
        switch (selected)
        {
            case "Pending approval": _viewModel.FilterPending(); break;
            case "Approved": _viewModel.FilterApproved(); break;
            case "Rejected": _viewModel.FilterRejected(); break;
            default: _viewModel.FilterAll(); break;
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e)
        => _viewModel.ToggleSort();

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        StatusFilterComboBox.SelectedIndex = 0;
        _viewModel.FilterAll();
    }

    private void DeleteRequestButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequestViewModel item = (NeighborhoodAccessRequestViewModel)((Button)sender).Tag;

        MessageBoxResult result = MessageBox.Show(
            "Are you sure you want to delete this request?",
            "Delete request",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;
        _viewModel.DeleteRequest(item.Id);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequestService requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        long? neighborhoodId = requestService.GetMembershipNeighborhoodId(_user.Id);
        if (neighborhoodId == null)
        {
            MessageBox.Show("You need to be a member of a neighborhood to view your profile.", "No Membership",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        NeighborhoodService neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        string neighborhoodName = neighborhoodService.GetNameById(neighborhoodId.Value) ?? "";
        new MyProfilePage(_user, neighborhoodId.Value, neighborhoodName).Show();
        Close();
    }

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
        => CitizenMenu.Visibility = Visibility.Visible;

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        switch (destination)
        {
            case "Neighborhoods":
                new BrowseNeighborhoodPage(_user).Show();
                Close();
                break;
            case "MyRequests": break;
            case "Events":
                NeighborhoodAccessRequestService requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
                long? neighborhoodId = requestService.GetMembershipNeighborhoodId(_user.Id);
                if (neighborhoodId == null)
                {
                    MessageBox.Show("You are not a member of any neighborhood.", "No Membership",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                new EventsPage(_user, neighborhoodId.Value).Show();
                Close();
                break;
            case "Citizens":
                NeighborhoodAccessRequestService rs = Injector.CreateInstance<NeighborhoodAccessRequestService>();
                long? nId = rs.GetMembershipNeighborhoodId(_user.Id);
                if (nId == null) { MessageBox.Show("You are not a member of any neighborhood."); return; }
                new NeighborhoodCitizensPage(_user, nId.Value).Show();
                Close();
                break;
            case "Meetings":
                NeighborhoodAccessRequestService meetingRequestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
                long? meetingNId = meetingRequestService.GetMembershipNeighborhoodId(_user.Id);
                if (meetingNId == null) { MessageBox.Show("You are not a member of any neighborhood."); return; }
                new MeetingsPage(_user, meetingNId.Value).Show();
                Close();
                break;
            case "CityObjects": MessageBox.Show("Go to City Objects page."); break;
            case "Budget": MessageBox.Show("Go to Budget page."); break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
}