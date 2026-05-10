using CommunityHub.Application.Domain.Buildings;

namespace CommunityHub.Application.Services.Buildings;

public class BuildingMembershipService
{
    private readonly IBuildingMembershipRepository _repository;

    public BuildingMembershipService(IBuildingMembershipRepository repository)
    {
        _repository = repository;
    }

    public List<BuildingMembership> GetByTenant(long tenantId)
    {
        return _repository.GetByTenant(tenantId);
    }
}