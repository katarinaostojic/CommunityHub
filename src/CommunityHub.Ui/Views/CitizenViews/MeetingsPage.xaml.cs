using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using System.Windows;
using System.Windows.Controls;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MeetingsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly MeetingsViewModel _viewModel;

    public MeetingsPage(User user, long neighborhoodId, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        MeetingService service = Injector.CreateInstance<MeetingService>();
        _viewModel = new MeetingsViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Meetings_Title") as string ?? "Meetings");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Meetings_Title") as string ?? "Meetings");
        NavBar.SetUrl("communityhub://meetings");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        NavBar.ReloadRequested += () => _viewModel.LoadMeetings();
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
    }

    private void VoteButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingVm = (MeetingViewModel)((Button)sender).Tag;
        VoteDateDialog dialog = new VoteDateDialog(meetingVm.Dto);
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
            _viewModel.Vote(meetingVm.Id, dialog.SelectedDate);
    }

    private void ChangeVoteButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingVm = (MeetingViewModel)((Button)sender).Tag;
        VoteDateDialog dialog = new VoteDateDialog(meetingVm.Dto);
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
            _viewModel.ChangeVote(meetingVm.CitizenVoteId!.Value, dialog.SelectedDate);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e) => NavigateToProfile();


    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, _neighborhoodId, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("Meetings", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "Meetings": CitizenMenu.Visibility = Visibility.Collapsed; break;
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
        var target = NavigationHistory.GoBack(new NavigationEntry("Meetings", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("Meetings", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
}