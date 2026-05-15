using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Application.Mappings.Buildings;

public static class CommonRoomRequestMappingExtensions
{
    public static CommonRoomRequestDto ToDto(this CommonRoomRequest request)
    {
        return new CommonRoomRequestDto(
            id: request.Id,
            commonRoomName: request.CommonRoom.Name,
            tenantFullName: $"{request.Tenant.Name} {request.Tenant.Surname}",
            dateFrom: request.DateFrom,
            dateTo: request.DateTo,
            status: request.Status,
            rentalType: request.CommonRoom.RentalType,
            approvedDate: request.ApprovedDate,
            proposedDateFrom: request.ProposedDateFrom,
            proposedDateTo: request.ProposedDateTo
        );
    }

    public static List<CommonRoomRequestDto> ToDtoList(this IEnumerable<CommonRoomRequest> requests)
    {
        return requests.Select(r => r.ToDto()).ToList();
    }
}