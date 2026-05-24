using System;
using System.Collections.Generic;
using System.Text;

using CommunityHub.Application.Domain.Entities.Neighborhoods;using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class NeighborhoodAccessRequestMappingExtensions
{
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
        => requests.Select(r => r.ToDto()).ToList();
}
