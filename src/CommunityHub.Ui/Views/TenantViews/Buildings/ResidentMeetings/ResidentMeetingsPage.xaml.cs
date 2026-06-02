using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ResidentMeetings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Dialogs.Buildings.ResidentMeetings;
using CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews.Buildings;

public partial class ResidentsMeetingsPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly ResidentsMeetingsViewModel _viewModel;

    public ResidentsMeetingsPage(User user, BuildingMembershipDto membership)
    {
        InitializeComponent();

        _user = user;
        _membership = membership;

        ResidentMeetingService meetingService = Injector.CreateInstance<ResidentMeetingService>();

        _viewModel = new ResidentsMeetingsViewModel(
            meetingService,
            user.Id,
            membership.BuildingId);

        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        BuildingInfoTextBlock.Text = GetBuildingInfo();
        AppMenu.Initialize(_user);
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterAll();

    private void FilterScheduledButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterScheduled();

    private void FilterConfirmedButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterConfirmed();

    private void FilterCancelledButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterCancelled();

    private void AttendButton_Click(object sender, RoutedEventArgs e)
    {
        ResidentMeetingCardViewModel meeting = GetSelectedMeeting(sender);

        _viewModel.Attend(meeting.Id);

        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            "✔ Attendance confirmed for your apartment.");
    }

    private void CancelAttendanceButton_Click(object sender, RoutedEventArgs e)
    {
        ResidentMeetingCardViewModel meeting = GetSelectedMeeting(sender);

        _viewModel.CancelAttendance(meeting.Id);

        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            "✔ Attendance cancelled for your apartment.");
    }

    private void SuggestTopicButton_Click(object sender, RoutedEventArgs e)
    {
        ResidentMeetingCardViewModel meeting = GetSelectedMeeting(sender);

        SuggestResidentMeetingTopicDialogViewModel dialogViewModel = new(
            GetBuildingInfo(),
            meeting.DateDisplay,
            meeting.TimeDisplay);

        SuggestResidentMeetingTopicDialog dialog = new(dialogViewModel)
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.SuggestTopic(meeting.Id, dialog.Topic);

        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            "✔ Topic suggestion sent successfully.");
    }

    private static ResidentMeetingCardViewModel GetSelectedMeeting(object sender)
    {
        return (ResidentMeetingCardViewModel)((Button)sender).Tag;
    }

    private string GetBuildingInfo()
    {
        return $"{_membership.BuildingStreet} {_membership.BuildingStreetNumber}, {_membership.BuildingNeighborhood}";
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}