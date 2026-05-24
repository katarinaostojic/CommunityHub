using CommunityHub.Application.DTOs.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Interfaces.Buildings.CommonRooms;

public interface ICommonRoomRequestService
{
    List<CommonRoomRequestDto> GetRequestsByCommonRoom(long commonRoomId);

    CommonRoomRequestDto? GetById(long requestId);

    List<DateTime> GetFreeDaysInRange(long requestId);

    List<(DateTime, DateTime)> FindAlternativeRanges(long requestId);

    void ApproveWithDate(long requestId, DateTime selectedDate);

    void RejectRequest(long requestId);

    void ProposeAlternative(long requestId, int alternativeIndex);

    List<CommonRoomRequestDto> GetByTenantAndBuilding(long tenantId, long buildingId);

    void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo);

    void CancelRequest(long requestId);

    void AcceptProposedDateChange(long requestId);
}