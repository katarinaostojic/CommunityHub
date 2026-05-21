using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class EventsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly EventsViewModel _viewModel;

    public EventsPage(User user, long neighborhoodId)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        EventService service = Injector.CreateInstance<EventService>();
        _viewModel = new EventsViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
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
            MessageBox.Show("You are already registered for this event.", "Already Registered",
                MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (!eventVm.CanRegister)
        {
            MessageBox.Show("This event is no longer accepting registrations.", "Cannot Join",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        JoinEventDialog dialog = new JoinEventDialog(_user.Id, eventVm.Event, _viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void ViewCitizensButton_Click(object sender, RoutedEventArgs e)
    {
        new NeighborhoodCitizensPage(_user, _neighborhoodId).Show();
        Close();
    }

    private void ViewMeetingsButton_Click(object sender, RoutedEventArgs e)
    {
        new MeetingsPage(_user, _neighborhoodId).Show();
        Close();
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string neighborhoodName = ns.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, neighborhoodName).Show();
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
            case "MyRequests": new MyRequestsPage(_user).Show(); Close(); break;
            case "Events": break;
            case "Citizens": new NeighborhoodCitizensPage(_user, _neighborhoodId).Show(); Close(); break;
            case "Meetings": new MeetingsPage(_user, _neighborhoodId).Show(); Close(); break;
            case "Profile": NavigateToProfile(); break;
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

    private void NavigateToProfile()
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, name).Show();
        Close();
    }
}