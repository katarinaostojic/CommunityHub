namespace CommunityHub.Application.Domain.Buildings;

public interface IBuildingMembershipRepository
{
    List<BuildingMembership> GetByTenant(long tenantId);
    void Create(BuildingAccessRequest request);
}