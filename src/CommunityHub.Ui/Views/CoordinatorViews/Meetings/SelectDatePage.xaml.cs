using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class SelectDatePage : Page
{
    private readonly MeetingsViewModel _meetingsViewModel;
    private readonly long _meetingId;
    private readonly long _coordinatorId;
    private readonly long _neighborhoodId;

    public SelectDatePage(MeetingsViewModel meetingsViewModel, long meetingId,
        long coordinatorId, long neighborhoodId, Dictionary<DateOnly, int> voteCounts)
    {
        InitializeComponent();
        _meetingsViewModel = meetingsViewModel;
        _meetingId = meetingId;
        _coordinatorId = coordinatorId;
        _neighborhoodId = neighborhoodId;

        foreach (var kvp in voteCounts.OrderByDescending(v => v.Value))
        {
            DateOptionsListBox.Items.Add(new ListBoxItem
            {
                Content = $"{kvp.Key:dd.MM.yyyy} — {kvp.Value} votes",
                Tag = kvp.Key
            });
        }
    }

    private void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (DateOptionsListBox.SelectedItem is ListBoxItem item)
        {
            DateOnly selectedDate = (DateOnly)item.Tag;
            _meetingsViewModel.FinalizeWithDate(_meetingId, selectedDate);
            CoordinatorMainWindow.Instance.NavigateTo(
                new MeetingsPage(_coordinatorId, _neighborhoodId), "Meetings");
        }
        else
        {
            DateErrorText.Text = "Please select a date.";
            DateErrorText.Visibility = Visibility.Visible;
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingsPage(_coordinatorId, _neighborhoodId), "Meetings");
    }
}