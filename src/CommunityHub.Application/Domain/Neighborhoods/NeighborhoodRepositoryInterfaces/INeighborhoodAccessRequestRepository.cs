using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

public interface INeighborhoodAccessRequestRepository
{
    void Create(User citizen, Neighborhood neighborhood);
    bool HasExistingPendingRequest(User citizen, Neighborhood neighborhood);
    bool HasMembership(long citizenId);
    List<NeighborhoodAccessRequest> GetAllByCitizen(long citizenId, string? status, bool sortDescending);
    int CountByCitizenAndStatus(long citizenId, string? status);
    void Delete(long requestId);
    void Update(NeighborhoodAccessRequest request);
    void CreateMembership(NeighborhoodAccessRequest request);
    List<NeighborhoodAccessRequest> GetAllByCoordinator(long coordinatorId, string? status, bool sortDescending);
}