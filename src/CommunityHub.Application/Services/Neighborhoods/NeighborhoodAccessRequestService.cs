using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

namespace CommunityHub.Application.Services.Neighborhoods;

public class NeighborhoodAccessRequestService
{
    private readonly INeighborhoodAccessRequestRepository _repository;

    public NeighborhoodAccessRequestService(INeighborhoodAccessRequestRepository repository)
    {
        _repository = repository;
    }

    public List<NeighborhoodAccessRequest> GetAllByCitizen(long citizenId, string? status, bool sortDescending)
        => _repository.GetAllByCitizen(citizenId, status, sortDescending);

    public int CountByCitizenAndStatus(long citizenId, string? status)
        => _repository.CountByCitizenAndStatus(citizenId, status);

    public void Delete(long id)
        => _repository.Delete(id);

    public List<NeighborhoodAccessRequest> GetAllByCoordinator(long coordinatorId, string? status, bool sortDescending)
        => _repository.GetAllByCoordinator(coordinatorId, status, sortDescending);

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

        if (neighborhood.ContainsAddress(citizen.Address ?? string.Empty))
        {
            NeighborhoodAccessRequest request = new NeighborhoodAccessRequest(citizen, neighborhood);
            _repository.CreateMembership(request);
            return AccessRequestResult.Granted;
        }

        _repository.Create(citizen, neighborhood);
        return AccessRequestResult.RequestCreated;
    }
}