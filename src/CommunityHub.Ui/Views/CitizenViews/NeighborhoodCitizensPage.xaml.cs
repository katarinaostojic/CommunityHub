using System.Windows;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class NeighborhoodCitizensPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly NeighborhoodCitizensViewModel _viewModel;

    public NeighborhoodCitizensPage(User user, long neighborhoodId)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        TrustRecordService service = Injector.CreateInstance<TrustRecordService>();
        _viewModel = new NeighborhoodCitizensViewModel(service, neighborhoodId);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;

        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
    }

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
        => CitizenMenu.Visibility = Visibility.Visible;

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodService neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        string neighborhoodName = neighborhoodService.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, neighborhoodName).Show();
        Close();
    }

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
            case "MyRequests":
                new MyRequestsPage(_user).Show();
                Close();
                break;
            case "Events":
                NeighborhoodAccessRequestService requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
                long? nId = requestService.GetMembershipNeighborhoodId(_user.Id);
                if (nId == null) { MessageBox.Show("You are not a member of any neighborhood."); return; }
                new EventsPage(_user, nId.Value).Show();
                Close();
                break;
            case "Citizens": break;
            case "Profile":
                NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
                string nbName = ns.GetNameById(_neighborhoodId) ?? "";
                new MyProfilePage(_user, _neighborhoodId, nbName).Show();
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