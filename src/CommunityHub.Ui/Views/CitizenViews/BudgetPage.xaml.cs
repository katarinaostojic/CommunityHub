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
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using CommunityHub.Ui.Views;
using CommunityHub.Application.Services.Entities.Neighborhoods.Budget;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class BudgetPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly BudgetViewModel _viewModel;

    public BudgetPage(User user, long neighborhoodId)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        DonationService service = Injector.CreateInstance<DonationService>();
        _viewModel = new BudgetViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
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

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
        => CitizenNavigationHelper.NavigateToProfile(_user, this);

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
            case "Profile": CitizenNavigationHelper.NavigateToProfile(_user, this); break;
            case "CityObjects": CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "Budget": break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }
}
