using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class CityObjectsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly CityObjectsViewModel _viewModel;

    public CityObjectsPage(User user, long neighborhoodId, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        CityObjectService service = Injector.CreateInstance<CityObjectService>();
        _viewModel = new CityObjectsViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("CityObjects_Title") as string ?? "CityObjects");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("CityObjects_Title") as string ?? "CityObjects");
        NavBar.SetUrl("communityhub://city-objects");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        NavBar.ReloadRequested += () => _viewModel.LoadCityObjects();
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
    }

    private void VoteButton_Click(object sender, RoutedEventArgs e)
    {
        CityObjectItemViewModel item = (CityObjectItemViewModel)((Button)sender).Tag;
        _viewModel.ToggleVote(item.Id);
    }

    private void ViewStatisticsButton_Click(object sender, RoutedEventArgs e)
    {
        StatisticsDialog dialog = new StatisticsDialog(_viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }



    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                NavigateToProfile(); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, _neighborhoodId, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("CityObjects", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "CityObjects": CitizenMenu.Visibility = Visibility.Collapsed; break;
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
        CitizenNavigationHelper.NavigateToProfile(_user, this);
    }
    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("CityObjects", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("CityObjects", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
}