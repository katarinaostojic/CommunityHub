using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Entities.Buildings.CommonRooms;

public class CommonRoomRequestCommandService
{
    private readonly ICommonRoomRepository _commonRoomRepository;
    private readonly ICommonRoomRequestRepository _requestRepository;
    private readonly IBuildingMembershipRepository _membershipRepository;
    private readonly CommonRoomRequestApprovalService _approvalService;

    public CommonRoomRequestCommandService(
        ICommonRoomRepository commonRoomRepository,
        ICommonRoomRequestRepository requestRepository,
        IBuildingMembershipRepository membershipRepository,
        CommonRoomRequestApprovalService approvalService)
    {
        _commonRoomRepository = commonRoomRepository;
        _requestRepository = requestRepository;
        _membershipRepository = membershipRepository;
        _approvalService = approvalService;
    }

    public void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
        CommonRoom room = GetRequiredCommonRoom(commonRoomId);
        EnsureTenantHasBuildingMembership(tenantId, room.BuildingId);
        ValidateDateRange(dateFrom, dateTo);

        long requestId = _requestRepository.Create(commonRoomId, tenantId, dateFrom, dateTo);
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
            return;

        ProcessCreatedRequest(request);
    }

    public void RejectRequest(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
            return;

        request.Reject();
        _requestRepository.Update(request);
    }

    public void CancelRequest(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
            return;

        request.EnsureCanBeCancelled();
        _requestRepository.Delete(request.Id);
    }

    public void AcceptProposedDateChange(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
            return;

        request.AcceptProposedDates();
        _requestRepository.Update(request);
        _approvalService.TryAutoApproveMultiDay(request);
    }

    private void ProcessCreatedRequest(CommonRoomRequest request)
    {
        if (request.CommonRoom.IsPerDayRental)
        {
            _approvalService.GetFreeDaysInRange(request.Id);
            return;
        }

        _approvalService.TryAutoApproveMultiDay(request);
    }

    private CommonRoom GetRequiredCommonRoom(long commonRoomId)
    {
        return _commonRoomRepository.GetById(commonRoomId)
            ?? throw new InvalidOperationException("Common room was not found.");
    }

    private void EnsureTenantHasBuildingMembership(long tenantId, long buildingId)
    {
        if (!_membershipRepository.Exists(tenantId, buildingId))
            throw new InvalidOperationException("Tenant is not a member of this building.");
    }

    private static void ValidateDateRange(DateTime dateFrom, DateTime dateTo)
    {
        string? validationError = CommonRoomRequest.ValidateDateRange(dateFrom, dateTo);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }
}