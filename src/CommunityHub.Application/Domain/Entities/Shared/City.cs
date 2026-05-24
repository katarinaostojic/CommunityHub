namespace CommunityHub.Application.Domain.Entities.Shared;

public class City
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public Country Country { get; private set; }


    public City(long id, string name, Country country)
    {
        Id = id;
        Name = name;
        Country = country;
    }

    public City(string name, Country country)
    {
        Id = 0;
        Name = name;
        Country = country;
    }
}
