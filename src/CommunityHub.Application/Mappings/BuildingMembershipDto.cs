using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Mappings.Buildings;

public static class BuildingMembershipMappingExtensions
{
    public static BuildingMembershipDto ToDto(this BuildingMembership membership)
    {
        return new BuildingMembershipDto(
            id: membership.Id,
            buildingId: membership.Building.Id,
            buildingStreet: membership.Building.Street,
            buildingStreetNumber: membership.Building.StreetNumber,
            buildingNeighborhood: membership.Building.Neighborhood,
            buildingCityName: membership.Building.City.Name,
            buildingCountryName: membership.Building.City.Country.Name,
            unitNumber: membership.UnitNumber,
            floorNumber: membership.FloorNumber,
            approvedAt: membership.ApprovedAt
        );
    }

    public static List<BuildingMembershipDto> ToDtoList(this IEnumerable<BuildingMembership> memberships)
    {
        return memberships.Select(m => m.ToDto()).ToList();
    }
}
