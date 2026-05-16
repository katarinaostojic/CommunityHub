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
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MyProfilePage : Window
{
    private readonly User _user;
    private readonly MyProfileViewModel _viewModel;

    public MyProfilePage(User user, long neighborhoodId, string neighborhoodName)
    {
        InitializeComponent();
        _user = user;

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
            case "Profile": break;
            case "Citizens": MessageBox.Show("Go to Citizens page."); break;
            case "Meetings": MessageBox.Show("Go to Meetings page."); break;
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
