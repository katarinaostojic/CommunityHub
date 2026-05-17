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
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MeetingsPage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly MeetingsViewModel _viewModel;

    public MeetingsPage(User user, long neighborhoodId)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        MeetingService service = Injector.CreateInstance<MeetingService>();
        _viewModel = new MeetingsViewModel(service, neighborhoodId, user.Id);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
    }

    private void VoteButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingVm = (MeetingViewModel)((Button)sender).Tag;
        VoteDateDialog dialog = new VoteDateDialog(meetingVm.Dto);
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
        {
            _viewModel.Vote(meetingVm.Id, dialog.SelectedDate);
        }
    }

    private void ChangeVoteButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingVm = (MeetingViewModel)((Button)sender).Tag;
        VoteDateDialog dialog = new VoteDateDialog(meetingVm.Dto);
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
        {
            _viewModel.ChangeVote(meetingVm.CitizenVoteId!.Value, dialog.SelectedDate);
        }
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e)
    {
        NeighborhoodService neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        string neighborhoodName = neighborhoodService.GetNameById(_neighborhoodId) ?? "";
        new MyProfilePage(_user, _neighborhoodId, neighborhoodName).Show();
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
            case "Neighborhoods":
                new BrowseNeighborhoodPage(_user).Show();
                Close();
                break;
            case "MyRequests":
                new MyRequestsPage(_user).Show();
                Close();
                break;
            case "Events":
                new EventsPage(_user, _neighborhoodId).Show();
                Close();
                break;
            case "Citizens":
                new NeighborhoodCitizensPage(_user, _neighborhoodId).Show();
                Close();
                break;
            case "Meetings": break;
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
