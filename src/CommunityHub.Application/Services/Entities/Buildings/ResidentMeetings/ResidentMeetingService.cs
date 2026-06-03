using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;
using CommunityHub.Application.DTOs.Buildings.ResidentMeetings;
using CommunityHub.Application.Mappings.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingService
{
    private readonly IResidentMeetingRepository _meetingRepository;
    private readonly IBuildingMembershipRepository _membershipRepository;

    public ResidentMeetingService(
        IResidentMeetingRepository meetingRepository,
        IBuildingMembershipRepository membershipRepository)
    {
        _meetingRepository = meetingRepository;
        _membershipRepository = membershipRepository;
    }

    public List<ResidentMeetingDto> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status = null)
    {
        EnsureTenantHasBuildingMembership(tenantId, buildingId);
        RefreshMeetingStatuses(buildingId);

        return _meetingRepository
            .GetByTenantAndBuilding(tenantId, buildingId, status)
            .ToDtoList(DateTime.Now);
    }

    public int CountByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ResidentMeetingStatus? status = null)
    {
        EnsureTenantHasBuildingMembership(tenantId, buildingId);
        RefreshMeetingStatuses(buildingId);

        return _meetingRepository.CountByTenantAndBuilding(
            tenantId,
            buildingId,
            status);
    }

    public void Attend(long meetingId, long tenantId)
    {
        ResidentMeeting meeting = GetMeetingForTenant(meetingId, tenantId);
        BuildingMembership membership = GetTenantMembership(
            tenantId,
            meeting.BuildingId);

        meeting.EnsureAttendanceCanBeChanged(DateTime.Now);

        ResidentMeetingAttendance? existingAttendance = _meetingRepository
            .GetAttendance(meetingId, membership.UnitNumber);

        if (existingAttendance != null)
            return;

        ResidentMeetingAttendance attendance = new(
            meetingId,
            tenantId,
            membership.UnitNumber);

        _meetingRepository.CreateAttendance(attendance);
        RefreshMeetingStatus(meetingId, tenantId);
    }

    public void CancelAttendance(long meetingId, long tenantId)
    {
        ResidentMeeting meeting = GetMeetingForTenant(meetingId, tenantId);
        BuildingMembership membership = GetTenantMembership(
            tenantId,
            meeting.BuildingId);

        meeting.EnsureAttendanceCanBeChanged(DateTime.Now);

        _meetingRepository.DeleteAttendance(meetingId, membership.UnitNumber);
        RefreshMeetingStatus(meetingId, tenantId);
    }

    public void SuggestTopic(CreateResidentMeetingTopicSuggestionDto request)
    {
        ResidentMeeting meeting = GetMeetingForTenant(
            request.MeetingId,
            request.TenantId);

        meeting.EnsureTopicCanBeSuggested(DateTime.Now);
        ValidateTopic(request.Topic);

        ResidentMeetingTopicSuggestion suggestion = new(
            request.MeetingId,
            request.TenantId,
            request.Topic);

        _meetingRepository.CreateTopicSuggestion(suggestion);
    }

    private void RefreshMeetingStatuses(long buildingId)
    {
        List<ResidentMeeting> meetings = _meetingRepository
            .GetActiveByBuilding(buildingId);

        foreach (ResidentMeeting meeting in meetings)
        {
            UpdateStatusIfNeeded(meeting);
        }
    }

    private void RefreshMeetingStatus(long meetingId, long tenantId)
    {
        ResidentMeeting meeting = GetMeetingForTenant(meetingId, tenantId);

        UpdateStatusIfNeeded(meeting);
    }

    private void UpdateStatusIfNeeded(ResidentMeeting meeting)
    {
        DateTime now = DateTime.Now;

        if (!meeting.ShouldUpdateStatus(now))
            return;

        _meetingRepository.UpdateStatus(
            meeting.Id,
            meeting.ResolveCurrentStatus(now));
    }

    private ResidentMeeting GetMeetingForTenant(long meetingId, long tenantId)
    {
        ResidentMeeting meeting = _meetingRepository.GetById(meetingId, tenantId)
            ?? throw new InvalidOperationException("Residents' meeting was not found.");

        EnsureTenantHasBuildingMembership(tenantId, meeting.BuildingId);

        return meeting;
    }

    private BuildingMembership GetTenantMembership(long tenantId, long buildingId)
    {
        return _membershipRepository
            .GetByTenant(tenantId)
            .FirstOrDefault(m => m.Building.Id == buildingId)
            ?? throw new InvalidOperationException("Tenant is not a member of this building.");
    }

    private void EnsureTenantHasBuildingMembership(long tenantId, long buildingId)
    {
        _ = GetTenantMembership(tenantId, buildingId);
    }

    private static void ValidateTopic(string topic)
    {
        string? validationError = ResidentMeeting.ValidateTopic(topic);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }

    public List<ResidentMeetingDto> GetAllByBuilding(long buildingId, ResidentMeetingStatus? status = null)
    {
        RefreshMeetingStatuses(buildingId);
        return _meetingRepository
            .GetAllByBuilding(buildingId, status)
            .ToDtoList(DateTime.Now);
    }

    public int CountByBuilding(long buildingId, ResidentMeetingStatus? status = null)
    {
        return _meetingRepository.CountByBuilding(buildingId, status);
    }

    public void CreateMeeting(long buildingId, DateTime date, TimeSpan time, List<string> topics)
    {
        if (_meetingRepository.HasConflict(buildingId, date, time))
            throw new InvalidOperationException("A meeting is already scheduled for this date and time.");

        long meetingId = _meetingRepository.CreateMeeting(buildingId, date, time);

        foreach (string topic in topics)
            _meetingRepository.AddTopic(meetingId, topic.Trim());
    }

    public List<ResidentMeetingTopicSuggestion> GetTopicSuggestions(long meetingId)
    {
        return _meetingRepository.GetTopicSuggestions(meetingId);
    }

    public void AddTopicFromSuggestion(long meetingId, string topic)
    {
        string? error = ResidentMeeting.ValidateTopic(topic);
        if (error != null)
            throw new InvalidOperationException(error);

        _meetingRepository.AddTopic(meetingId, topic.Trim());
    }

    public List<ResidentMeetingAttendance> GetAttendances(long meetingId)
    {
        return _meetingRepository.GetAttendances(meetingId);
    }
}