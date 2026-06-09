using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ResidentMeetings;

public class ResidentMeetingCardViewModel : BaseViewModel
{
    private readonly ResidentMeetingDto _meeting;
    private readonly string _selectedUnitNumber;

    public ResidentMeetingCardViewModel(ResidentMeetingDto meeting, string selectedUnitNumber)
    {
        _meeting = meeting;
        _selectedUnitNumber = selectedUnitNumber;
    }

    public long Id => _meeting.Id;
    public ResidentMeetingStatus Status => _meeting.Status;
    public List<string> Topics => _meeting.Topics;

    public string DateDisplay => _meeting.MeetingDate.ToString("dd.MM.yyyy.");
    public string TimeDisplay => _meeting.MeetingTime.ToString(@"hh\:mm");
    public string DeadlineDisplay => $"Changes allowed until {_meeting.DeadlineAt:dd.MM.yyyy. HH:mm}";
    public string AttendanceDisplay => $"{_meeting.AttendanceCount}/{_meeting.UnitCount} apartments attending";
    public string AttendancePercentageDisplay => $"{_meeting.AttendancePercentage:0}% confirmed";
    public string TopicsCountDisplay => $"{_meeting.Topics.Count} topic{(_meeting.Topics.Count == 1 ? "" : "s")}";

    public bool CanAttend => _meeting.CanAttend;
    public bool CanCancelAttendance => _meeting.CanCancelAttendance;
    public bool CanSuggestTopic => _meeting.CanSuggestTopic;
    public bool HasTopics => _meeting.Topics.Count > 0;

    public string StatusDisplay => _meeting.Status switch
    {
        ResidentMeetingStatus.Scheduled => "Scheduled",
        ResidentMeetingStatus.Confirmed => "Confirmed",
        ResidentMeetingStatus.Cancelled => "Cancelled",
        _ => _meeting.Status.ToString()
    };

    public string AttendanceActionText => _meeting.IsTenantAttending
        ? $"You confirmed attendance for apartment {_selectedUnitNumber}."
        : $"Apartment {_selectedUnitNumber} has not confirmed attendance yet.";
}