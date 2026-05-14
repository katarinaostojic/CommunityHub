using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Ui.Mappings;

public static class BuildingMappingExtensions
{
    public static BuildingDto ToDto(this Building building)
    {
        return new BuildingDto(
            id: building.Id,
            street: building.Street,
            streetNumber: building.StreetNumber,
            neighborhood: building.Neighborhood,
            cityName: building.City.Name,
            countryName: building.City.Country.Name,
            numberOfFloors: building.NumberOfFloors,
            totalUnits: building.TotalUnits,
            vacancyCount: building.VacancyCount,
            imagePaths: building.Images.Select(i => i.Path).ToList()
        );
    }

    public static List<BuildingDto> ToDtoList(this IEnumerable<Building> buildings)
    {
        return buildings.Select(b => b.ToDto()).ToList();
    }
}