using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Services.Entities.Neighborhoods.Reviews;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class CoordinatorReviewsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly long _coordinatorId;
    private readonly string _coordinatorName;
    private readonly CoordinatorReviewsViewModel _viewModel;

    public CoordinatorReviewsPage(User user, long neighborhoodId, long coordinatorId, string coordinatorName, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;
        _coordinatorId = coordinatorId;
        _coordinatorName = coordinatorName;

        CoordinatorReviewService service = Injector.CreateInstance<CoordinatorReviewService>();
        _viewModel = new CoordinatorReviewsViewModel(service, neighborhoodId, user.Id, coordinatorId, coordinatorName);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Reviews_Title") as string ?? "CoordinatorReviews");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Reviews_Title") as string ?? "CoordinatorReviews");
        NavBar.SetUrl("communityhub://coordinator-reviews");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        NavBar.ReloadRequested += () => _viewModel.LoadReviews();
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
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
            MessageBox.Show(error, MsgHelper.Get("Msg_Error", "Greška"), MessageBoxButton.OK, MessageBoxImage.Warning);
    }



    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "CoordinatorReviews": CitizenMenu.Visibility = Visibility.Collapsed; break;
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
        var target = NavigationHistory.GoBack(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("CoordinatorReviews", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
}