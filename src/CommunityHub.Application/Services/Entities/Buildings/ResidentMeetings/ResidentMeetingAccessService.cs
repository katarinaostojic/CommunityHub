using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ResidentMeetings;

namespace CommunityHub.Application.Services.Entities.Buildings.ResidentMeetings;

public class ResidentMeetingAccessService
{
    private readonly IResidentMeetingRepository _meetingRepository;
    private readonly IBuildingMembershipRepository _membershipRepository;

    public ResidentMeetingAccessService(
        IResidentMeetingRepository meetingRepository,
        IBuildingMembershipRepository membershipRepository)
    {
        _meetingRepository = meetingRepository;
        _membershipRepository = membershipRepository;
    }

    public ResidentMeeting GetMeetingForTenant(long meetingId, long tenantId)
    {
        ResidentMeeting meeting = _meetingRepository.GetById(meetingId, tenantId)
            ?? throw new InvalidOperationException("Residents' meeting was not found.");

        EnsureTenantHasBuildingMembership(tenantId, meeting.BuildingId);

        return meeting;
    }

    public BuildingMembership GetTenantMembership(long tenantId, long buildingId)
    {
        return _membershipRepository
            .GetByTenant(tenantId)
            .FirstOrDefault(m => m.Building.Id == buildingId)
            ?? throw new InvalidOperationException("Tenant is not a member of this building.");
    }

    public BuildingMembership GetTenantMembershipForUnit(
        long tenantId,
        long buildingId,
        string unitNumber)
    {
        return _membershipRepository
            .GetByTenant(tenantId)
            .FirstOrDefault(m =>
                m.Building.Id == buildingId
                && m.UnitNumber == unitNumber)
            ?? throw new InvalidOperationException("Tenant is not a member of this apartment.");
    }

    public void EnsureTenantHasBuildingMembership(long tenantId, long buildingId)
    {
        _ = GetTenantMembership(tenantId, buildingId);
    }
}