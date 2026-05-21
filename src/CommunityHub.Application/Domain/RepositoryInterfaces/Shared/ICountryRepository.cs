using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Shared;

public interface ICountryRepository
{
    Country Create(Country country);
    List<Country> GetAll();
    Country? GetById(long id);
    void Update(Country country);
    bool Delete(long id);
}