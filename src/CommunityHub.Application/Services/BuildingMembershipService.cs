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

    public List<BuildingMembership> GetByBuilding(long buildingId)
    {
        return _repository.GetByBuilding(buildingId);
    }

    public bool IsUnitOccupied(long buildingId, string unitNumber)
    {
        return _repository.ExistsForUnit(buildingId, unitNumber);
    }

    public List<string> GetOccupiedUnits(long buildingId)
    {
        return _repository.GetOccupiedUnits(buildingId);
    }
}