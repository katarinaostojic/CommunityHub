using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Views.CitizenViews.Dialogs;
using System.Windows;
using System.Windows.Controls;

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
            _viewModel.Vote(meetingVm.Id, dialog.SelectedDate);
    }

    private void ChangeVoteButton_Click(object sender, RoutedEventArgs e)
    {
        MeetingViewModel meetingVm = (MeetingViewModel)((Button)sender).Tag;
        VoteDateDialog dialog = new VoteDateDialog(meetingVm.Dto);
        dialog.Owner = this;
        if (dialog.ShowDialog() == true)
            _viewModel.ChangeVote(meetingVm.CitizenVoteId!.Value, dialog.SelectedDate);
    }

    private void ProfileButton_Click(object sender, RoutedEventArgs e) => NavigateToProfile();

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
            case "Meetings": break;
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