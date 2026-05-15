using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class NeighborhoodMappingExtensions
{
    public static NeighborhoodDto ToDto(this Neighborhood neighborhood)
    {
        return new NeighborhoodDto(
            id: neighborhood.Id,
            name: neighborhood.Name,
            description: neighborhood.Description,
            cityName: neighborhood.Location.CityName,
            countryName: neighborhood.Location.CountryName,
            coordinatorId: neighborhood.CoordinatorId,
            imagePaths: neighborhood.Images.Select(i => i.Path).ToList()
        );
    }

    public static List<NeighborhoodDto> ToDtoList(this IEnumerable<Neighborhood> neighborhoods)
    {
        return neighborhoods.Select(n => n.ToDto()).ToList();
    }

    public static NeighborhoodMembershipDto ToDto(this NeighborhoodMembership membership)
    {
        return new NeighborhoodMembershipDto(
            id: membership.Id,
            citizenName: membership.Citizen.Name,
            citizenSurname: membership.Citizen.Surname,
            citizenAddress: membership.Citizen.Address,
            joinedAt: membership.JoinedAt
        );
    }

    public static List<NeighborhoodMembershipDto> ToDtoList(this IEnumerable<NeighborhoodMembership> memberships)
    {
        return memberships.Select(m => m.ToDto()).ToList();
    }
    public static NeighborhoodAccessRequestDto ToDto(this NeighborhoodAccessRequest request)
    {
        return new NeighborhoodAccessRequestDto(
            id: request.Id,
            citizenName: request.Citizen.Name,
            citizenSurname: request.Citizen.Surname,
            citizenAddress: request.Citizen.Address,
            neighborhoodName: request.Neighborhood.Name,
            createdAt: request.CreatedAt,
            status: request.Status,
            rejectionReason: request.RejectionReason
        );
    }

    public static List<NeighborhoodAccessRequestDto> ToDtoList(this IEnumerable<NeighborhoodAccessRequest> requests)
    {
        return requests.Select(r => r.ToDto()).ToList();
    }
}