using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;
using CommunityHub.Application.Mappings.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingService
{
    private readonly IResidentMeetingRepository _meetingRepository;
    private readonly ResidentMeetingAccessService _accessService;
    private readonly ResidentMeetingStatusService _statusService;
    private readonly ResidentMeetingAttendanceService _attendanceService;
    private readonly ResidentMeetingTopicService _topicService;
    private readonly ResidentMeetingScheduleService _scheduleService;

    public ResidentMeetingService(
        IResidentMeetingRepository meetingRepository,
        ResidentMeetingAccessService accessService,
        ResidentMeetingStatusService statusService,
        ResidentMeetingAttendanceService attendanceService,
        ResidentMeetingTopicService topicService,
        ResidentMeetingScheduleService scheduleService)
    {
        _meetingRepository = meetingRepository;
        _accessService = accessService;
        _statusService = statusService;
        _attendanceService = attendanceService;
        _topicService = topicService;
        _scheduleService = scheduleService;
    }

    public List<ResidentMeetingDto> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status = null)
    {
        DateTime now = CurrentTime;

        _accessService.EnsureTenantHasBuildingMembership(tenantId, buildingId);
        _statusService.RefreshBuildingMeetings(buildingId, now);

        return _meetingRepository
            .GetByTenantAndBuilding(tenantId, buildingId, status)
            .ToDtoList(now);
    }

    public int CountByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status = null)
    {
        _accessService.EnsureTenantHasBuildingMembership(tenantId, buildingId);
        _statusService.RefreshBuildingMeetings(buildingId, CurrentTime);

        return _meetingRepository.CountByBuilding(buildingId, status);
    }

    public void Attend(long meetingId, long tenantId)
    {
        _attendanceService.Attend(meetingId, tenantId, CurrentTime);
    }

    public void CancelAttendance(long meetingId, long tenantId)
    {
        _attendanceService.CancelAttendance(meetingId, tenantId, CurrentTime);
    }

    public void SuggestTopic(CreateResidentMeetingTopicSuggestionDto request)
    {
        _topicService.SuggestTopic(request, CurrentTime);
    }

    public List<ResidentMeetingDto> GetAllByBuilding(
        long buildingId,
        ResidentMeetingStatus? status = null)
    {
        DateTime now = CurrentTime;

        _statusService.RefreshBuildingMeetings(buildingId, now);

        return _meetingRepository
            .GetAllByBuilding(buildingId, status)
            .ToDtoList(now);
    }

    public int CountByBuilding(long buildingId, ResidentMeetingStatus? status = null)
    {
        _statusService.RefreshBuildingMeetings(buildingId, CurrentTime);

        return _meetingRepository.CountByBuilding(buildingId, status);
    }

    public void CreateMeeting(
        long buildingId,
        DateTime date,
        TimeSpan time,
        List<string> topics)
    {
        _scheduleService.CreateMeeting(buildingId, date, time, topics);
    }

    public List<ResidentMeetingTopicSuggestion> GetTopicSuggestions(long meetingId)
    {
        return _topicService.GetTopicSuggestions(meetingId);
    }

    public void AddTopicFromSuggestion(long meetingId, string topic)
    {
        _topicService.AddTopicFromSuggestion(meetingId, topic, CurrentTime);
    }

    public List<ResidentMeetingAttendance> GetAttendances(long meetingId)
    {
        return _meetingRepository.GetAttendances(meetingId);
    }

    private static DateTime CurrentTime => DateTime.Now;
}