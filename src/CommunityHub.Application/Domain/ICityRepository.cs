namespace CommunityHub.Application.Domain;

public interface ICityRepository
{
    City Create(City city);
    List<City> GetAll();
    City? GetById(long id);
    List<City> GetByCountry(long countryId);
    void Update(City city);
    void Delete(long id);
}