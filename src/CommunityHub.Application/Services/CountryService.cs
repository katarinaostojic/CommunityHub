namespace CommunityHub.Application.Services;

using CommunityHub.Application.Domain;

public class CountryService
{
    private readonly ICountryRepository _repository;

    public CountryService(ICountryRepository repository)
    {
        _repository = repository;
    }

    public Country Create(Country country) => _repository.Create(country);
    public List<Country> GetAll() => _repository.GetAll();
    public Country? GetById(long id) => _repository.GetById(id);
    public void Update(Country country) => _repository.Update(country);
    public bool Delete(long id) => _repository.Delete(id);
}