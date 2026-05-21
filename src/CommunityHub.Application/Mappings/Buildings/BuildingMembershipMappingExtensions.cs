using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Mappings.Buildings;

public static class BuildingMembershipMappingExtensions
{
    public static BuildingMembershipDto ToDto(this BuildingMembership membership)
    {
        return new BuildingMembershipDto(
            id: membership.Id,
            buildingId: membership.Building?.Id ?? 0,
            buildingStreet: GetBuildingStreet(membership),
            buildingStreetNumber: GetBuildingStreetNumber(membership),
            buildingNeighborhood: GetBuildingNeighborhood(membership),
            buildingCityName: GetBuildingCityName(membership),
            buildingCountryName: GetBuildingCountryName(membership),
            unitNumber: membership.UnitNumber,
            floorNumber: membership.FloorNumber,
            approvedAt: membership.ApprovedAt,
            tenantFullName: $"{membership.User.Name} {membership.User.Surname}"
        );
    }

    private static string GetBuildingStreet(BuildingMembership membership)
        => membership.Building?.Street ?? string.Empty;

    private static string GetBuildingStreetNumber(BuildingMembership membership)
        => membership.Building?.StreetNumber ?? string.Empty;

    private static string GetBuildingNeighborhood(BuildingMembership membership)
        => membership.Building?.Neighborhood ?? string.Empty;

    private static string GetBuildingCityName(BuildingMembership membership)
        => membership.Building?.City?.Name ?? string.Empty;

    private static string GetBuildingCountryName(BuildingMembership membership)
        => membership.Building?.City?.Country?.Name ?? string.Empty;

    public static List<BuildingMembershipDto> ToDtoList(this IEnumerable<BuildingMembership> memberships)
    {
        return memberships.Select(m => m.ToDto()).ToList();
    }
}