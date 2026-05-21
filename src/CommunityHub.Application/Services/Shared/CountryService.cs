namespace CommunityHub.Application.Services.Shared;

using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Domain.RepositoryInterfaces.Shared;

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