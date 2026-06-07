using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ResidentMeetings;

public class ResidentMeetingRowViewModel
{
    private readonly ResidentMeetingDto _dto;

    public ResidentMeetingRowViewModel(ResidentMeetingDto dto)
    {
        _dto = dto;
    }

    public long Id => _dto.Id;
    public long BuildingId => _dto.BuildingId;
    public List<string> Topics => _dto.Topics;
    public int AttendanceCount => _dto.AttendanceCount;
    public int UnitCount => _dto.UnitCount;

    public string DateDisplay => _dto.MeetingDate.ToString("dd.MM.yyyy.");
    public string TimeDisplay => _dto.MeetingTime.ToString(@"hh\:mm");
    public string AttendanceDisplay => $"{_dto.AttendanceCount}/{_dto.UnitCount} apartments confirmed";

    public string StatusDisplay => _dto.Status switch
    {
        ResidentMeetingStatus.Scheduled => "Scheduled",
        ResidentMeetingStatus.Confirmed => "Confirmed",
        ResidentMeetingStatus.Cancelled => "Cancelled",
        _ => _dto.Status.ToString()
    };

    public string StatusBackground => _dto.Status switch
    {
        ResidentMeetingStatus.Scheduled => "#FFF3CD",
        ResidentMeetingStatus.Confirmed => "#D4EDDA",
        ResidentMeetingStatus.Cancelled => "#F8D7DA",
        _ => "#F0F0F0"
    };

    public string StatusForeground => _dto.Status switch
    {
        ResidentMeetingStatus.Scheduled => "#856404",
        ResidentMeetingStatus.Confirmed => "#155724",
        ResidentMeetingStatus.Cancelled => "#721C24",
        _ => "#2C3E50"
    };

    // Manager može da vidi predloge stanara samo za scheduled i confirmed skupštine
    public bool CanViewSuggestions => _dto.Status != ResidentMeetingStatus.Cancelled;
}