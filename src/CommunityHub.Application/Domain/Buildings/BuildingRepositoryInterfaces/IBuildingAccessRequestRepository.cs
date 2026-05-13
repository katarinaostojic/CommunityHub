namespace CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;

public interface IBuildingAccessRequestRepository
{
    void Create(BuildingAccessRequest request);
    List<BuildingAccessRequest> GetAllByTenant(long tenantId, RequestStatus? status, bool sortDescending);
    List<BuildingAccessRequest> GetAllByManager(long managerId, string? status, bool sortDescending);
    int CountByTenantAndStatus(long tenantId, RequestStatus? status);
    int GetPendingRequestsCount(long buildingId);
    void Delete(long requestId);
    void Update(BuildingAccessRequest request);
}