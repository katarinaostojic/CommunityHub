using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Neighborhoods;

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

    public void Delete(long id)
        => _repository.Delete(id);

    public List<NeighborhoodAccessRequestDto> GetAllByCoordinator(long coordinatorId, string? status, bool sortDescending)
        => _repository.GetAllByCoordinator(coordinatorId, status, sortDescending).ToDtoList();

    public NeighborhoodAccessRequest? GetById(long id)
        => _repository.GetById(id);

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

    public AccessRequestResult RequestAccess(User citizen, Neighborhood neighborhood)
    {
        if (_repository.HasExistingPendingRequest(citizen, neighborhood))
            return AccessRequestResult.AlreadyPending;

        if (_repository.HasMembership(citizen.Id))
            return AccessRequestResult.AlreadyMember;

        _repository.Create(citizen, neighborhood);
        return AccessRequestResult.RequestCreated;
    }

    public AccessRequestResult RequestAccessById(User citizen, long neighborhoodId)
    {
        Neighborhood? neighborhood = _neighborhoodRepository.GetById(neighborhoodId);
        if (neighborhood == null)
            throw new InvalidOperationException("Neighborhood not found.");
        return RequestAccess(citizen, neighborhood);
    }

    public long? GetMembershipNeighborhoodId(long citizenId)
        => _repository.GetMembershipNeighborhoodId(citizenId);
}