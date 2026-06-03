using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class CoordinatorReviewsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly long _coordinatorId;
    private readonly string _coordinatorName;
    private readonly CoordinatorReviewsViewModel _viewModel;

    public CoordinatorReviewsPage(User user, long neighborhoodId, long coordinatorId, string coordinatorName)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;
        _coordinatorId = coordinatorId;
        _coordinatorName = coordinatorName;

        CoordinatorReviewService service = Injector.CreateInstance<CoordinatorReviewService>();
        _viewModel = new CoordinatorReviewsViewModel(service, neighborhoodId, user.Id, coordinatorId, coordinatorName);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
    }

    private void RateCoordinatorButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorReviewService service = Injector.CreateInstance<CoordinatorReviewService>();
        RateCoordinatorDialog dialog = new RateCoordinatorDialog(
            service, _user.Id, _coordinatorId, _neighborhoodId, _viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void ReportButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorReviewItemViewModel item = (CoordinatorReviewItemViewModel)((Button)sender).Tag;
        var (success, error) = _viewModel.ReportReview(item.Id);
        if (!success)
            MessageBox.Show(error, "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, name).Show();
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
            case "Neighborhoods": new BrowseNeighborhoodPage(_user).Show(); Close(); break;
            case "MyRequests": new MyRequestsPage(_user).Show(); Close(); break;
            case "Events": new EventsPage(_user, _neighborhoodId).Show(); Close(); break;
            case "Citizens": new NeighborhoodCitizensPage(_user, _neighborhoodId).Show(); Close(); break;
            case "Meetings": new MeetingsPage(_user, _neighborhoodId).Show(); Close(); break;
            case "Profile": NavigateToProfile(); break;
            case "CityObjects": CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "Budget": CitizenNavigationHelper.NavigateToBudget(_user, this); break;
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