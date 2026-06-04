using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingScheduleService
{
    private readonly IResidentMeetingRepository _meetingRepository;

    public ResidentMeetingScheduleService(IResidentMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public void CreateMeeting(
        long buildingId,
        DateTime date,
        TimeSpan time,
        List<string> topics)
    {
        if (_meetingRepository.HasConflict(buildingId, date, time))
            throw new InvalidOperationException("A meeting is already scheduled for this date and time.");

        long meetingId = _meetingRepository.CreateMeeting(buildingId, date, time);

        foreach (string topic in topics)
            _meetingRepository.AddTopic(meetingId, topic.Trim());
    }
}