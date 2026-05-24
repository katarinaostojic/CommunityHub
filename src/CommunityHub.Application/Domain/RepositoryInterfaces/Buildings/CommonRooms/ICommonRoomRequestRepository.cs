using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;

public interface ICommonRoomRequestRepository
{
    List<CommonRoomRequest> GetAllByCommonRoom(long commonRoomId);
    List<CommonRoomRequest> GetByTenant(long tenantId);
    List<CommonRoomRequest> GetByTenantAndBuilding(long tenantId, long buildingId);
    CommonRoomRequest? GetById(long requestId);
    long Create(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo);
    void Update(CommonRoomRequest request);
    void Delete(long requestId);
}