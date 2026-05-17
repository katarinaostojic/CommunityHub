using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Mappings.Buildings;

public static class BuildingMembershipMappingExtensions
{
    public static BuildingMembershipDto ToDto(this BuildingMembership membership)
    {
        return new BuildingMembershipDto(
            id: membership.Id,
            buildingId: membership.Building?.Id ?? 0,
            buildingStreet: membership.Building?.Street ?? string.Empty,
            buildingStreetNumber: membership.Building?.StreetNumber ?? string.Empty,
            buildingNeighborhood: membership.Building?.Neighborhood ?? string.Empty,
            buildingCityName: membership.Building?.City?.Name ?? string.Empty,
            buildingCountryName: membership.Building?.City?.Country?.Name ?? string.Empty,
            unitNumber: membership.UnitNumber,
            floorNumber: membership.FloorNumber,
            approvedAt: membership.ApprovedAt,
            tenantFullName: $"{membership.User.Name} {membership.User.Surname}"
        );
    }

    public static List<BuildingMembershipDto> ToDtoList(this IEnumerable<BuildingMembership> memberships)
    {
        return memberships.Select(m => m.ToDto()).ToList();
    }
}
