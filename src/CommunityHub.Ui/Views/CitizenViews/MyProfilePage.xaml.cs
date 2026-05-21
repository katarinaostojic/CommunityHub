using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using System.Windows;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MyProfilePage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly MyProfileViewModel _viewModel;

    public MyProfilePage(User user, long neighborhoodId, string neighborhoodName)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        TrustRecordService service = Injector.CreateInstance<TrustRecordService>();
        _viewModel = new MyProfileViewModel(service, user, neighborhoodId, neighborhoodName);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        UsernameText.Text = _user.Username;
        NeighborhoodNameText.Text = neighborhoodName;

        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
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
            case "Events": NavigateToEvents(); break;
            case "Citizens": NavigateToCitizens(); break;
            case "Meetings": NavigateToMeetings(); break;
            case "Profile": break;
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

    private void NavigateToEvents()
    {
        long? nId = GetMembershipId();
        if (nId == null) return;
        new EventsPage(_user, nId.Value).Show();
        Close();
    }

    private void NavigateToCitizens()
    {
        new NeighborhoodCitizensPage(_user, _neighborhoodId).Show();
        Close();
    }

    private void NavigateToMeetings()
    {
        long? nId = GetMembershipId();
        if (nId == null) return;
        new MeetingsPage(_user, nId.Value).Show();
        Close();
    }

    private long? GetMembershipId()
    {
        NeighborhoodAccessRequestService s = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        long? nId = s.GetMembershipNeighborhoodId(_user.Id);
        if (nId == null) MessageBox.Show("You are not a member of any neighborhood.");
        return nId;
    }
}