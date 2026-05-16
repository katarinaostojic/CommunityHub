using CommunityHub.Application.Domain.Buildings;

public interface ICommonRoomRequestRepository
{
    List<CommonRoomRequest> GetAllByCommonRoom(long commonRoomId);
    List<CommonRoomRequest> GetByTenant(long tenantId);
    CommonRoomRequest? GetById(long requestId);
    void Create(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo);
    void Update(CommonRoomRequest request);
    void Delete(long requestId);
}