using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Entities.Buildings.CommonRooms;

public class CommonRoomRequestCommandService
{
    private readonly ICommonRoomRequestRepository _requestRepository;
    private readonly CommonRoomRequestApprovalService _approvalService;

    public CommonRoomRequestCommandService(
        ICommonRoomRequestRepository requestRepository,
        CommonRoomRequestApprovalService approvalService)
    {
        _requestRepository = requestRepository;
        _approvalService = approvalService;
    }

    public void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
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

        if (!request.CanBeCancelled)
            throw new InvalidOperationException("Only pending requests can be cancelled.");

        _requestRepository.Delete(request.Id);
    }

    public void AcceptProposedDateChange(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null || !request.CanAcceptProposedDateChange)
            return;

        ValidateDateRange(request.ProposedDateFrom!.Value, request.ProposedDateTo!.Value);

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

    private static void ValidateDateRange(DateTime dateFrom, DateTime dateTo)
    {
        string? validationError = CommonRoomRequest.ValidateDateRange(dateFrom, dateTo);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }
}