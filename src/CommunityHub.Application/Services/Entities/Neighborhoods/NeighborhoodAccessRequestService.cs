using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

public class NeighborhoodAccessRequestService
{
    private readonly INeighborhoodAccessRequestRepository _repository;
    private readonly INeighborhoodRepository _neighborhoodRepository;

    public NeighborhoodAccessRequestService(
        INeighborhoodAccessRequestRepository repository,
        INeighborhoodRepository neighborhoodRepository)
    {
        _repository = repository;
        _neighborhoodRepository = neighborhoodRepository;
    }

    public List<NeighborhoodAccessRequestDto> GetAllByCitizen(long citizenId, string? status, bool sortDescending)
        => _repository.GetAllByCitizen(citizenId, status, sortDescending).ToDtoList();

    public int CountByCitizenAndStatus(long citizenId, string? status)
        => _repository.CountByCitizenAndStatus(citizenId, status);

    public void Delete(long requestId)
        => _repository.Delete(requestId);

    public List<NeighborhoodAccessRequestDto> GetAllByCoordinator(long coordinatorId, string? status, bool sortDescending)
        => _repository.GetAllByCoordinator(coordinatorId, status, sortDescending).ToDtoList();

    public NeighborhoodAccessRequest? GetById(long requestId)
        => _repository.GetById(requestId);

    public void ApproveRequestWithMembership(NeighborhoodAccessRequest request)
    {
        request.Approve();
        _repository.Update(request);
        _repository.CreateMembership(request);
    }

    public void RejectRequestForCoordinator(NeighborhoodAccessRequest request, string? rejectionReason)
    {
        request.Reject(rejectionReason);
        _repository.Update(request);
    }

    public AccessRequestResult RequestAccessById(long citizenId, long neighborhoodId)
    {
        if (_repository.HasExistingPendingRequest(citizenId, neighborhoodId))
            return AccessRequestResult.AlreadyPending;

        if (_repository.HasMembership(citizenId))
            return AccessRequestResult.AlreadyMember;

        _repository.Create(citizenId, neighborhoodId);
        return AccessRequestResult.RequestCreated;
    }

    public long? GetMembershipNeighborhoodId(long citizenId)
        => _repository.GetMembershipNeighborhoodId(citizenId);

    public int CountPendingByNeighborhood(long neighborhoodId)
    => _repository.CountPendingByNeighborhood(neighborhoodId);
}