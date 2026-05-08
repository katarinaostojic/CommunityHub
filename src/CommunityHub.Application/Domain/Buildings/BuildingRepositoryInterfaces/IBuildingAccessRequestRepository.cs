namespace CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;

public interface IBuildingAccessRequestRepository
{
    void Create(User user, Building building, string unitNumber);
    List<BuildingAccessRequest> GetAllByTenant(long tenantId, RequestStatus? status, bool sortDescending);
    List<BuildingAccessRequest> GetAllByManager(long managerId, string? status, bool sortDescending);
    int CountByTenantAndStatus(long tenantId, RequestStatus? status);
    int GetPendingRequestsCount(long buildingId);
    void Delete(long requestId);
    void ApproveRequest(long requestId);
    void RejectRequest(long requestId, string? rejectionReason);
}