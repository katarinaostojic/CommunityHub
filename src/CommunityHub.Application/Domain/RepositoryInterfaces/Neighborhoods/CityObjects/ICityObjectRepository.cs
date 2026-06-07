using CommunityHub.Application.Domain.Entities.Neighborhoods.CityObjects;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.CityObjects;

public interface ICityObjectRepository
{
    List<CityObject> GetAll();
    CityObject? GetById(long id);
    List<CityObjectReservation> GetReservations(long cityObjectId);
    void AddReservation(CityObjectReservation reservation);
    void ResetVotes(long cityObjectId, long neighborhoodId);
    List<CityObject> GetByNeighborhood(long neighborhoodId, long citizenId);
    bool HasVoted(long cityObjectId, long citizenId);
    void AddVote(long cityObjectId, long citizenId, long neighborhoodId);
    void RemoveVote(long cityObjectId, long citizenId);
    int GetVoteCount(long cityObjectId, long neighborhoodId);
    DateOnly? GetLastVisit(long cityObjectId, long neighborhoodId);
    List<CityObjectReservation> GetReservationsByNeighborhood(long neighborhoodId, int? month, int? year);
    List<CityObjectReservation> GetReservationsByCityObject(long cityObjectId, long neighborhoodId);

    List<CityObject> GetAllByNeighborhood(long neighborhoodId);
}
