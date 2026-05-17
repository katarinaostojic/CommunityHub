using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.DTOs.Neighborhoods;

public class MeetingDto
{
    public long Id { get; init; }
    public string Theme { get; init; }
    public string MeetingTime { get; init; }
    public string DateRangeStart { get; init; }
    public string DateRangeEnd { get; init; }
    public string Status { get; init; }
    public string? ScheduledDate { get; init; }
    public bool CanVote { get; init; }
    public DateOnly? CitizenVotedDate { get; init; }
    public long? CitizenVoteId { get; init; }

    public MeetingDto(long id, string theme, string meetingTime, string dateRangeStart,
        string dateRangeEnd, string status, string? scheduledDate, bool canVote,
        DateOnly? citizenVotedDate, long? citizenVoteId)
    {
        Id = id;
        Theme = theme;
        MeetingTime = meetingTime;
        DateRangeStart = dateRangeStart;
        DateRangeEnd = dateRangeEnd;
        Status = status;
        ScheduledDate = scheduledDate;
        CanVote = canVote;
        CitizenVotedDate = citizenVotedDate;
        CitizenVoteId = citizenVoteId;
    }

    public bool IsInPreparation => Status == "in_preparation";
    public bool IsScheduled => Status == "scheduled";
    public bool HasVoted => CitizenVotedDate != null;
    public string CitizenVotedDateFormatted => CitizenVotedDate?.ToString("dd/MM/yyyy") ?? "";
    public string StatusDisplay => Status switch
    {
        "in_preparation" => "⏳ In Preparation",
        "scheduled" => "✔ Scheduled",
        "cancelled" => "✕ Cancelled",
        _ => Status
    };

    public string ThemeDisplay => Theme switch
    {
        "welcome" => "🤝 Welcome",
        "motivation" => "💪 Motivation",
        _ => Theme
    };
}
