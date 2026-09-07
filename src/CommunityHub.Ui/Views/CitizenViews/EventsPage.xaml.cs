using CommunityHub.Application.DependencyInjection;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Events;
using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class EventsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly EventsViewModel _viewModel;

    public EventsPage(User user, long neighborhoodId, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        EventService service = Injector.CreateInstance<EventService>();
        _viewModel = new EventsViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Events_Title") as string ?? "Events");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Events_Title") as string ?? "Events");
        NavBar.SetUrl("communityhub://events");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        NavBar.ReloadRequested += () => _viewModel.LoadEvents();

        CitizenMenu.CloseRequested += () => CitizenMenu.Visibility = Visibility.Collapsed;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
    }

    private void CreateEventButton_Click(object sender, RoutedEventArgs e)
    {
        CreateEventDialog dialog = new CreateEventDialog(_user.Id, _neighborhoodId, _viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        EventViewModel eventVm = (EventViewModel)((Button)sender).Tag;
        if (_viewModel.IsAlreadyRegistered(eventVm.Id, _user.Id))
        {
            MsgHelper.Info("Msg_AlreadyRegistered", "Msg_AlreadyRegisteredTitle");
            return;
        }
        if (!eventVm.CanRegister)
        {
            MsgHelper.Warn("Msg_RegistrationsClosed", "Msg_RegistrationsClosedTitle");
            return;
        }
        JoinEventDialog dialog = new JoinEventDialog(_user.Id, eventVm.Event, _viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void ViewCitizensButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
        new NeighborhoodCitizensPage(_user, _neighborhoodId, openMenuOnLoad: true).Show();
        Close();
    }

    private void ViewMeetingsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
        new MeetingsPage(_user, _neighborhoodId, openMenuOnLoad: true).Show();
        Close();
    }

    private void CoordinatorReviewsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        var (coordinatorId, coordinatorName) = ns.GetCoordinatorInfo(_neighborhoodId);
        new CoordinatorReviewsPage(_user, _neighborhoodId, coordinatorId, coordinatorName, openMenuOnLoad: true).Show();
        Close();
    }

    private void MarkAttendedButton_Click(object sender, RoutedEventArgs e)
    {
        AttendanceItemViewModel item = (AttendanceItemViewModel)((Button)sender).Tag;
        _viewModel.MarkAttendance(item.RegistrationId, true);
    }

    private void MarkAbsentButton_Click(object sender, RoutedEventArgs e)
    {
        AttendanceItemViewModel item = (AttendanceItemViewModel)((Button)sender).Tag;
        _viewModel.MarkAttendance(item.RegistrationId, false);
    }

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                CitizenMenu.Visibility = Visibility.Collapsed; break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                new NeighborhoodCitizensPage(_user, _neighborhoodId, openMenuOnLoad: true).Show(); Close(); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                new MeetingsPage(_user, _neighborhoodId, openMenuOnLoad: true).Show(); Close(); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, _neighborhoodId, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("Events", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        NavigationHistory.Clear();
        new LogInForm().Show();
        Close();
    }

    private void NavigateToProfile()
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, name, openMenuOnLoad: true).Show();
        Close();
    }

    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("Events", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("Events", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
}
