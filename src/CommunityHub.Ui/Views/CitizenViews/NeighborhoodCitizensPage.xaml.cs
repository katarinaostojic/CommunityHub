using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using System.Windows;

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

    private void ProfileButton_Click(object sender, RoutedEventArgs e) => NavigateToProfile();

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        switch (destination)
        {
            case "Neighborhoods": new BrowseNeighborhoodPage(_user).Show(); Close(); break;
            case "MyRequests": new MyRequestsPage(_user).Show(); Close(); break;
            case "Events": new EventsPage(_user, _neighborhoodId).Show(); Close(); break;
            case "Citizens": break;
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