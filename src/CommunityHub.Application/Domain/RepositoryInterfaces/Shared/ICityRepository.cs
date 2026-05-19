using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Shared;

public interface ICityRepository
{
    City Create(City city);
    List<City> GetAll();
    City? GetById(long id);
    List<City> GetByCountry(long countryId);
    void Update(City city);
    void Delete(long id);
}