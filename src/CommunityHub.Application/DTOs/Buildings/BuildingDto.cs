namespace CommunityHub.Application.DTOs.Buildings;

public class BuildingDto
{
    public long Id { get; init; }
    public string Street { get; init; }
    public string StreetNumber { get; init; }
    public string Neighborhood { get; init; }
    public string CityName { get; init; }
    public string CountryName { get; init; }
    public int NumberOfFloors { get; init; }
    public int TotalUnits { get; init; }
    public int VacancyCount { get; init; }
    public List<BuildingMembershipDto> Memberships { get; init; }
    public List<string> ImagePaths { get; init; }

    public BuildingDto(
        long id,
        string street,
        string streetNumber,
        string neighborhood,
        string cityName,
        string countryName,
        int numberOfFloors,
        int totalUnits,
        int vacancyCount,
        List<BuildingMembershipDto> memberships,
        List<string> imagePaths)
    {
        Id = id;
        Street = street;
        StreetNumber = streetNumber;
        Neighborhood = neighborhood;
        CityName = cityName;
        CountryName = countryName;
        NumberOfFloors = numberOfFloors;
        TotalUnits = totalUnits;
        VacancyCount = vacancyCount;
        Memberships = memberships;
        ImagePaths = imagePaths;
    }

    public string FullAddress => $"{Street} {StreetNumber}";
    public string Location => $"{CityName}, {CountryName}";
}