using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;

public interface IBuildingAccessRequestRepository
{
    void Create(BuildingAccessRequest request);
    BuildingAccessRequest? GetById(long requestId);
    List<BuildingAccessRequest> GetAllByTenant(long tenantId, RequestStatus? status, bool sortDescending);
    List<BuildingAccessRequest> GetAllByManager(long managerId, string? status, bool sortDescending);
    int CountByTenantAndStatus(long tenantId, RequestStatus? status);
    int GetPendingRequestsCount(long buildingId);
    void Delete(long requestId);
    void Update(BuildingAccessRequest request);
}