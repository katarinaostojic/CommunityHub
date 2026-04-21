using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services;

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