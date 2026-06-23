using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using CommunityHub.Application.Services.Entities.Neighborhoods.Budget;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class BudgetPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly BudgetViewModel _viewModel;

    public BudgetPage(User user, long neighborhoodId, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        DonationService service = Injector.CreateInstance<DonationService>();
        _viewModel = new BudgetViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Budget_Title") as string ?? "Budget");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Budget_Title") as string ?? "Budget");
        NavBar.SetUrl("communityhub://budget");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => NavigateToProfile();
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        NavBar.ReloadRequested += () => _viewModel.LoadData();
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
    }

    private void DonateButton_Click(object sender, RoutedEventArgs e)
    {
        DonateDialog dialog = new DonateDialog(_viewModel);
        dialog.Owner = this;
        dialog.ShowDialog();
    }

    private void SpendingHistoryButton_Click(object sender, RoutedEventArgs e)
    {
        SpendingHistoryDialog dialog = new SpendingHistoryDialog(_viewModel);
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
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "Profile":
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                NavigateToProfile(); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("Budget", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "Budget": CitizenMenu.Visibility = Visibility.Collapsed; break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("Budget", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("Budget", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
    private void NavigateToProfile()
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, name, openMenuOnLoad: true).Show();
        Close();
    }

}