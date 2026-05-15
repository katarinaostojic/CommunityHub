using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class AddNewMeetingViewModel : BaseViewModel
{
    private readonly MeetingService _meetingService;
    private readonly long _neighborhoodId;

    public AddNewMeetingViewModel(MeetingService meetingService, long neighborhoodId)
    {
        _meetingService = meetingService;
        _neighborhoodId = neighborhoodId;
    }

    public void CreateMeeting(MeetingTheme theme, TimeOnly meetingTime, DateOnly startDate, DateOnly endDate)
    {
        Meeting meeting = new Meeting(_neighborhoodId, theme, meetingTime, startDate, endDate);
        _meetingService.CreateMeeting(meeting);
    }
}