using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Services.Interfaces.Buildings;

public interface IBuildingAccessRequestService
{
    void CreateForBuilding(long buildingId, User tenant, string unitNumber);

    List<BuildingAccessRequestDto> GetAllByTenant(long tenantId, RequestStatus? status, bool sortDescending);

    List<BuildingAccessRequestDto> GetAllByManager(long managerId, string? status, bool sortDescending);

    int CountByTenantAndStatus(long tenantId, RequestStatus? status);

    int GetPendingRequestsCount(long buildingId);

    void Cancel(long requestId);

    void Delete(long id);

    void ApproveRequest(BuildingAccessRequest request);

    void RejectRequest(BuildingAccessRequest request, string? rejectionReason);

    BuildingAccessRequest? GetById(long requestId);
}