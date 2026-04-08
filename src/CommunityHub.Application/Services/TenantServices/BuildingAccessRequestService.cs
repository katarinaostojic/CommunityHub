using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Building;

namespace CommunityHub.Application.Services.TenantServices;

public class BuildingAccessRequestService
{
    private readonly BuildingAccessRequestDbRepository _repository;

    public BuildingAccessRequestService()
    {
        _repository = new BuildingAccessRequestDbRepository();
    }

    public List<BuildingAccessRequest> GetAllByTenant(long tenantId, string? status, bool sortDescending)
    {
        return _repository.GetAllByTenant(tenantId, status, sortDescending);
    }

    public int CountByTenantAndStatus(long tenantId, string? status)
    {
        return _repository.CountByTenantAndStatus(tenantId, status);
    }

    public void Delete(long id)
    {
        _repository.Delete(id);
    }

    public void Create(long userId, long buildingId, string unitNumber)
    {
        _repository.Create(userId, buildingId, unitNumber);
    }
    public bool HasExistingRequest(long userId, long buildingId, string unitNumber)
    {
        return _repository.HasExistingRequest(userId, buildingId, unitNumber);
    }

    public bool IsUnitOccupied(long buildingId, string unitNumber)
    {
        return _repository.IsUnitOccupied(buildingId, unitNumber);
    }
    public int GetPendingRequestsCount(long buildingId)
    {
        return _repository.GetPendingRequestsCount(buildingId);
    }
}