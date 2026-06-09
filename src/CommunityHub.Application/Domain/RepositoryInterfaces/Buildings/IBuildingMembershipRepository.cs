using CommunityHub.Application.Domain.Entities.Buildings;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;

public interface IBuildingMembershipRepository
{
    List<BuildingMembership> GetByTenant(long tenantId);
    bool Exists(long tenantId, long buildingId);
    void Create(BuildingAccessRequest request);
}