using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Mappings.Buildings;

namespace CommunityHub.Application.Services.Buildings;

public class BuildingAccessRequestService
{
    private readonly IBuildingAccessRequestRepository _repository;
    private readonly IBuildingMembershipRepository _membershipRepository;
    private readonly IBuildingRepository _buildingRepository;

    public BuildingAccessRequestService(
        IBuildingAccessRequestRepository repository,
        IBuildingMembershipRepository membershipRepository,
        IBuildingRepository buildingRepository)
    {
        _repository = repository;
        _membershipRepository = membershipRepository;
        _buildingRepository = buildingRepository;
    }

    public void CreateForBuilding(long buildingId, User tenant, string unitNumber)
    {
        Building building = _buildingRepository.GetById(buildingId)
            ?? throw new InvalidOperationException("Building not found.");

        ValidateAccessRequest(building, tenant.Id, unitNumber);

        BuildingAccessRequest request = new BuildingAccessRequest(tenant, building, unitNumber);
        _repository.Create(request);
    }

    public List<BuildingAccessRequestDto> GetAllByTenant(long tenantId, RequestStatus? status, bool sortDescending)
    {
        return _repository.GetAllByTenant(tenantId, status, sortDescending).ToDtoList();
    }

    public List<BuildingAccessRequestDto> GetAllByManager(long managerId, string? status, bool sortDescending)
    {
        return _repository.GetAllByManager(managerId, status, sortDescending).ToDtoList();
    }

    public int CountByTenantAndStatus(long tenantId, RequestStatus? status)
    {
        return _repository.CountByTenantAndStatus(tenantId, status);
    }

    public int GetPendingRequestsCount(long buildingId)
    {
        return _repository.GetPendingRequestsCount(buildingId);
    }

    public void Cancel(long requestId)
    {
        BuildingAccessRequest request = _repository.GetById(requestId)
            ?? throw new InvalidOperationException("Request not found.");

        request.Cancel();
        _repository.Delete(requestId);
    }

    public void Delete(long id)
    {
        Cancel(id);
    }

    public void ApproveRequest(BuildingAccessRequest request)
    {
        request.Approve();
        _repository.Update(request);
        _membershipRepository.Create(request);
    }

    public void RejectRequest(BuildingAccessRequest request, string? rejectionReason)
    {
        request.Reject(rejectionReason);
        _repository.Update(request);
    }

    public BuildingAccessRequest? GetById(long requestId)
    {
        return _repository.GetById(requestId);
    }

    private void ValidateAccessRequest(Building building, long tenantId, string unitNumber)
    {
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new InvalidOperationException("Apartment number is required.");

        if (!building.ContainsUnit(unitNumber))
            throw new InvalidOperationException("Apartment does not exist in this building.");

        if (building.HasExistingRequest(tenantId, unitNumber))
            throw new InvalidOperationException("You already have a pending request for this apartment.");
    }
}