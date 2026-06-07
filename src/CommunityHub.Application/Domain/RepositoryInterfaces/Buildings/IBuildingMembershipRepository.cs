using CommunityHub.Application.Domain.Entities.Buildings;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;

public interface IBuildingMembershipRepository
{
    List<BuildingMembership> GetByTenant(long tenantId);
    void Create(BuildingAccessRequest request);
}