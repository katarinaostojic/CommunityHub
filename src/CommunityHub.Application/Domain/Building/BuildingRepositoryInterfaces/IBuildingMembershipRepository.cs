namespace CommunityHub.Application.Domain.Building;

public interface IBuildingMembershipRepository
{
    List<BuildingMembership> GetByTenant(long tenantId);
    void Create(BuildingAccessRequest request);
}