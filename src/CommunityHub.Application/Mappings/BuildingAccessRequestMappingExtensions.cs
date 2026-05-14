using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Mappings.Buildings;

public static class BuildingAccessRequestMappingExtensions
{
    public static BuildingAccessRequestDto ToDto(this BuildingAccessRequest request)
    {
        return new BuildingAccessRequestDto(
            id: request.Id,
            buildingId: request.Building.Id,
            buildingStreet: request.Building.Street,
            buildingStreetNumber: request.Building.StreetNumber,
            buildingNeighborhood: request.Building.Neighborhood,
            buildingCityName: request.Building.City.Name,
            buildingCountryName: request.Building.City.Country.Name,
            unitNumber: request.UnitNumber,
            createdAt: request.CreatedAt,
            status: request.Status,
            rejectionReason: request.RejectionReason
        );
    }

    public static List<BuildingAccessRequestDto> ToDtoList(this IEnumerable<BuildingAccessRequest> requests)
    {
        return requests.Select(r => r.ToDto()).ToList();
    }
}
