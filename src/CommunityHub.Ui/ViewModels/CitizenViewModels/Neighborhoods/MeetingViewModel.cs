using CommunityHub.Ui.Helpers.Citizen;
using CommunityHub.Application.DTOs.Neighborhoods.Meetings;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class MeetingViewModel : BaseViewModel
{
    private readonly MeetingDto _dto;

    private static readonly Dictionary<string, string> StatusColorMap = new()
    {
        ["in_preparation"] = "#F39C12",
        ["scheduled"] = "#27AE60",
        ["cancelled"] = "#C0392B"
    };

    public MeetingViewModel(MeetingDto dto)
    {
        _dto = dto;
    }

    public long Id => _dto.Id;
    public string Theme => _dto.ThemeDisplay;
    public string MeetingTime => _dto.MeetingTime;
    public string DateRangeStart => _dto.DateRangeStart;
    public string DateRangeEnd => _dto.DateRangeEnd;
    public string StatusDisplay => _dto.Status switch
    {
        "in_preparation" => System.Windows.Application.Current.TryFindResource("Meeting_Status_InPreparation") as string ?? "⏳ In Preparation",
        "scheduled" => System.Windows.Application.Current.TryFindResource("Meeting_Status_Scheduled") as string ?? "✔ Scheduled",
        "cancelled" => System.Windows.Application.Current.TryFindResource("Meeting_Status_Cancelled") as string ?? "✕ Cancelled",
        _ => _dto.Status
    };
    public string? ScheduledDate => _dto.ScheduledDate;
    public bool CanVote => _dto.CanVote;
    public bool HasVoted => _dto.HasVoted;
    public string CitizenVotedDate => _dto.CitizenVotedDateFormatted;
    public long? CitizenVoteId => _dto.CitizenVoteId;
    public MeetingDto Dto => _dto;

    public Visibility VoteVisibility => CanVote && !HasVoted
        ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ChangeVoteVisibility => CanVote && HasVoted
        ? Visibility.Visible : Visibility.Collapsed;

    public Visibility ScheduledDateVisibility => _dto.IsScheduled
        ? Visibility.Visible : Visibility.Collapsed;

    public Visibility DateRangeVisibility => !_dto.IsScheduled
        ? Visibility.Visible : Visibility.Collapsed;

    public Visibility VotedDateVisibility => HasVoted
        ? Visibility.Visible : Visibility.Collapsed;

    public SolidColorBrush StatusBrush
    {
        get
        {
            string hex = StatusColorMap.TryGetValue(_dto.Status, out var color) ? color : "#7F8C8D";
            return new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));
        }
    }
    public void RefreshStatus() => OnPropertyChanged(nameof(StatusDisplay));

}
