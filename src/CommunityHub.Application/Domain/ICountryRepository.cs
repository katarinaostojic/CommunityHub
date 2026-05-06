namespace CommunityHub.Application.Domain;

public interface ICountryRepository
{
    Country Create(Country country);
    List<Country> GetAll();
    Country? GetById(long id);
    void Update(Country country);
    bool Delete(long id);
}