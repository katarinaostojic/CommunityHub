namespace CommunityHub.Application.Services.Shared;

using CommunityHub.Application.Domain;

public class CityService
{
    private readonly ICityRepository _repository;

    public CityService(ICityRepository repository)
    {
        _repository = repository;
    }

    public City Create(City city) => _repository.Create(city);
    public List<City> GetAll() => _repository.GetAll();
    public City? GetById(long id) => _repository.GetById(id);
    public List<City> GetByCountry(long countryId) => _repository.GetByCountry(countryId);
    public void Update(City city) => _repository.Update(city);
    public void Delete(long id) => _repository.Delete(id);
}