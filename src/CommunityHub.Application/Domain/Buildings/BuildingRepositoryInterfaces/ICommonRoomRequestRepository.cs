using CommunityHub.Application.Domain.Buildings;

public interface ICommonRoomRequestRepository
{
    List<CommonRoomRequest> GetAllByCommonRoom(long commonRoomId);
    CommonRoomRequest? GetById(long requestId);
    void Update(CommonRoomRequest request);
}