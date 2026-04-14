using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services;

public class BuildingMembershipService
{
    private readonly BuildingMembershipDbRepository _repository;

    public BuildingMembershipService()
    {
        _repository = new BuildingMembershipDbRepository();
    }

    public List<BuildingMembership> GetByTenant(long tenantId)
    {
        return _repository.GetByTenant(tenantId);
    }

}