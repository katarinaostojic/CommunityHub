using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Domain.Building.BuildingRepositoryInterfaces;

namespace CommunityHub.Application.Services;

public class BuildingAccessRequestService
{
    private readonly IBuildingAccessRequestRepository _repository;
    private readonly IBuildingMembershipRepository _membershipRepository;

    public BuildingAccessRequestService(
        IBuildingAccessRequestRepository repository,
        IBuildingMembershipRepository membershipRepository)
    {
        _repository = repository;
        _membershipRepository = membershipRepository;
    }

    public void Create(User user, Building building, string unitNumber)
    {
        _repository.Create(user, building, unitNumber);
    }

    public List<BuildingAccessRequest> GetAllByTenant(long tenantId, string? status, bool sortDescending)
    {
        return _repository.GetAllByTenant(tenantId, status, sortDescending);
    }

    public List<BuildingAccessRequest> GetAllByManager(long managerId, string? status, bool sortDescending)
    {
        return _repository.GetAllByManager(managerId, status, sortDescending);
    }

    public int CountByTenantAndStatus(long tenantId, string? status)
    {
        return _repository.CountByTenantAndStatus(tenantId, status);
    }

    public int GetPendingRequestsCount(long buildingId)
    {
        return _repository.GetPendingRequestsCount(buildingId);
    }

    public void Delete(long id)
    {
        _repository.Delete(id);
    }

    public void ApproveRequest(BuildingAccessRequest request)
    {
        _repository.ApproveRequest(request.Id);
        _membershipRepository.Create(request);
    }

    public void RejectRequest(long requestId, string? rejectionReason)
    {
        _repository.RejectRequest(requestId, rejectionReason);
    }
}