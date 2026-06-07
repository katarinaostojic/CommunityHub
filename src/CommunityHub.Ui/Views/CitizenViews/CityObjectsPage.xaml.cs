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

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class CityObjectsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly CityObjectsViewModel _viewModel;

    public CityObjectsPage(User user, long neighborhoodId)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        CityObjectService service = Injector.CreateInstance<CityObjectService>();
        _viewModel = new CityObjectsViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
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
            case "Events": CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens": CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings": CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
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
        CitizenNavigationHelper.NavigateToProfile(_user, this);
    }
}
