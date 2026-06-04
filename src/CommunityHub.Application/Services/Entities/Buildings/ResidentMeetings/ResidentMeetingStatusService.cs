using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingStatusService
{
    private readonly IResidentMeetingRepository _meetingRepository;

    public ResidentMeetingStatusService(IResidentMeetingRepository meetingRepository)
    {
        _meetingRepository = meetingRepository;
    }

    public void RefreshBuildingMeetings(long buildingId, DateTime now)
    {
        List<ResidentMeeting> meetings = _meetingRepository
            .GetActiveByBuilding(buildingId);

        foreach (ResidentMeeting meeting in meetings)
            UpdateStatusIfNeeded(meeting, now);
    }

    public void RefreshMeetingForTenant(long meetingId, long tenantId, DateTime now)
    {
        ResidentMeeting? meeting = _meetingRepository.GetById(meetingId, tenantId);

        if (meeting == null)
            return;

        UpdateStatusIfNeeded(meeting, now);
    }

    private void UpdateStatusIfNeeded(ResidentMeeting meeting, DateTime now)
    {
        if (!meeting.ShouldUpdateStatus(now))
            return;

        _meetingRepository.UpdateStatus(
            meeting.Id,
            meeting.ResolveCurrentStatus(now));
    }
}