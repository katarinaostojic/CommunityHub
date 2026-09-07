using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using System;
using System.Windows;
using System.Windows.Controls;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MyRequestsPage : Window
{
    private readonly User _user;
    private readonly MyRequestsViewModel _viewModel;

    public MyRequestsPage(User user, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;

        NeighborhoodAccessRequestService service = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        _viewModel = new MyRequestsViewModel(service, user.Id);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Requests_Title") as string ?? "MyRequests");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Requests_Title") as string ?? "MyRequests");
        NavBar.SetUrl("communityhub://my-requests");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;

        // Refresh ComboBox labela kada se jezik promeni
        LanguageManager.LanguageChanged += RefreshFilterComboBox;
        NavBar.ReloadRequested += () => _viewModel.FilterAll();
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
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
        MessageBoxResult result = MsgHelper.Confirm("Msg_DeleteConfirm", "Msg_DeleteTitle");
        if (result != MessageBoxResult.Yes) return;
        _viewModel.DeleteRequest(item.Id);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e) => CitizenNavigationHelper.NavigateToProfile(_user, this);


    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, CitizenNavigationHelper.GetMembershipId(_user.Id) ?? 0, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("MyRequests", _user));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "MyRequests": CitizenMenu.Visibility = Visibility.Collapsed; break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("MyRequests", _user));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("MyRequests", _user));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
    private void NavigateToProfile()
        => CitizenNavigationHelper.NavigateToProfile(_user, this);

    private void RefreshFilterComboBox()
    {
        int idx = StatusFilterComboBox.SelectedIndex;
        StatusFilterComboBox.Items.Clear();
        var items = new[]
        {
            TryFindResource("Requests_All")      as string ?? "Svi",
            TryFindResource("Requests_Pending")  as string ?? "Na čekanju",
            TryFindResource("Requests_Approved") as string ?? "Odobreni",
            TryFindResource("Requests_Rejected") as string ?? "Odbijeni",
        };
        foreach (var item in items)
            StatusFilterComboBox.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = item });
        StatusFilterComboBox.SelectedIndex = idx >= 0 ? idx : 0;
    }

}