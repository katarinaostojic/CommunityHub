using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
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
        int selectedIndex = StatusFilterComboBox.SelectedIndex;
        switch (selectedIndex)
        {
            case 1: _viewModel.FilterPending(); break;
            case 2: _viewModel.FilterApproved(); break;
            case 3: _viewModel.FilterRejected(); break;
            default: _viewModel.FilterAll(); break;
        }
    }

    private void SortButton_Click(object sender, RoutedEventArgs e) => _viewModel.ToggleSort();

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        StatusFilterComboBox.SelectedIndex = 0;
        _viewModel.FilterAll();
    }

    private void DeleteRequestButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodAccessRequestViewModel item = (NeighborhoodAccessRequestViewModel)((Button)sender).Tag;
        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this request?",
            "Delete request", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes) return;
        _viewModel.DeleteRequest(item.Id);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e) => CitizenNavigationHelper.NavigateToProfile(_user, this);

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
        => CitizenMenu.Visibility = Visibility.Visible;

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        switch (destination)
        {
            case "Neighborhoods": new BrowseNeighborhoodPage(_user).Show(); Close(); break;
            case "MyRequests": break;
            case "Events": CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens": CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings": CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile": CitizenNavigationHelper.NavigateToProfile(_user, this); break;
            case "CityObjects": CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
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