using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using System.Windows;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class NeighborhoodCitizensPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly NeighborhoodCitizensViewModel _viewModel;

    public NeighborhoodCitizensPage(User user, long neighborhoodId, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        TrustRecordService service = Injector.CreateInstance<TrustRecordService>();
        _viewModel = new NeighborhoodCitizensViewModel(service, neighborhoodId);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Citizens_Title") as string ?? "Neighborhood Citizens");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Citizens_Title") as string ?? "Neighborhood Citizens");
        NavBar.SetUrl("communityhub://citizens");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
    }


    private void ProfileButton_Click(object sender, RoutedEventArgs e) => NavigateToProfile();

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("NeighborhoodCitizens", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, _neighborhoodId, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("Citizens", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "Citizens": CitizenMenu.Visibility = Visibility.Collapsed; break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }

    private void NavigateToProfile()
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, name).Show();
        Close();
    }
    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("Citizens", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("Citizens", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
}