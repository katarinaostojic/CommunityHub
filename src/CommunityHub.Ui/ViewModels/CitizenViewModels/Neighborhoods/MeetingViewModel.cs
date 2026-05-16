using System;
using System.Collections.Generic;
using System.Text;

using CommunityHub.Application.DTOs.Neighborhoods;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class MeetingViewModel : BaseViewModel
{
    private readonly MeetingDto _dto;

    public MeetingViewModel(MeetingDto dto)
    {
        _dto = dto;
    }

    public long Id => _dto.Id;
    public string Theme => _dto.ThemeDisplay;
    public string MeetingTime => _dto.MeetingTime;
    public string DateRangeStart => _dto.DateRangeStart;
    public string DateRangeEnd => _dto.DateRangeEnd;
    public string StatusDisplay => _dto.StatusDisplay;
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
}
