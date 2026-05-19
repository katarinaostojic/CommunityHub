namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface INeighborhoodAccessRequestRepository
{
    void Create(long citizenId, long neighborhoodId);
    bool HasExistingPendingRequest(long citizenId, long neighborhoodId);
    bool HasMembership(long citizenId);
    List<NeighborhoodAccessRequest> GetAllByCitizen(long citizenId, string? status, bool sortDescending);
    int CountByCitizenAndStatus(long citizenId, string? status);
    void Delete(long requestId);
    void Update(NeighborhoodAccessRequest request);
    void CreateMembership(NeighborhoodAccessRequest request);
    List<NeighborhoodAccessRequest> GetAllByCoordinator(long coordinatorId, string? status, bool sortDescending);
    long? GetMembershipNeighborhoodId(long citizenId);
    NeighborhoodAccessRequest? GetById(long id);
}