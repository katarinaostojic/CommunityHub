namespace CommunityHub.Application.Domain;

public class Location
{
    public long Id { get; private set; }
    public string CityName { get; private set; }
    public string CountryName { get; private set; }

    public Location(long id, string cityName, string countryName)
    {
        Id = id;
        CityName = cityName;
        CountryName = countryName;
    }
}