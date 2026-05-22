using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class NeighborhoodMembershipMappingExtensions
{
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
        => memberships.Select(m => m.ToDto()).ToList();
}