using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingAttendanceService
{
    private readonly IResidentMeetingRepository _meetingRepository;
    private readonly ResidentMeetingAccessService _accessService;
    private readonly ResidentMeetingStatusService _statusService;

    public ResidentMeetingAttendanceService(
        IResidentMeetingRepository meetingRepository,
        ResidentMeetingAccessService accessService,
        ResidentMeetingStatusService statusService)
    {
        _meetingRepository = meetingRepository;
        _accessService = accessService;
        _statusService = statusService;
    }

    public void Attend(long meetingId, long tenantId, DateTime now)
    {
        ResidentMeeting meeting = _accessService.GetMeetingForTenant(meetingId, tenantId);
        BuildingMembership membership = _accessService.GetTenantMembership(
            tenantId,
            meeting.BuildingId);

        meeting.EnsureAttendanceCanBeChanged(now);

        if (HasAttendance(meetingId, membership.UnitNumber))
            return;

        ResidentMeetingAttendance attendance = new(
            meetingId,
            tenantId,
            membership.UnitNumber);

        _meetingRepository.CreateAttendance(attendance);
        _statusService.RefreshMeetingForTenant(meetingId, tenantId, now);
    }

    public void CancelAttendance(long meetingId, long tenantId, DateTime now)
    {
        ResidentMeeting meeting = _accessService.GetMeetingForTenant(meetingId, tenantId);
        BuildingMembership membership = _accessService.GetTenantMembership(
            tenantId,
            meeting.BuildingId);

        meeting.EnsureAttendanceCanBeChanged(now);

        _meetingRepository.DeleteAttendance(meetingId, membership.UnitNumber);
        _statusService.RefreshMeetingForTenant(meetingId, tenantId, now);
    }

    private bool HasAttendance(long meetingId, string unitNumber)
    {
        return _meetingRepository.GetAttendance(meetingId, unitNumber) != null;
    }
}