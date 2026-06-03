using CommunityHub.Application.Domain.Entities.Neighborhoods;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface ICityObjectRepository
{
    List<CityObject> GetAll();
    CityObject? GetById(long id);
    List<CityObjectReservation> GetReservations(long cityObjectId);
    void AddReservation(CityObjectReservation reservation);
    void ResetVotes(long cityObjectId, long neighborhoodId);
}
