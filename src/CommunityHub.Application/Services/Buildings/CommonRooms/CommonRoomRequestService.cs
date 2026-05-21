using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Mappings.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Buildings.CommonRooms;

public class CommonRoomRequestService
{
    private readonly ICommonRoomRequestRepository _requestRepository;
    private readonly CommonRoomRequestApprovalService _approvalService;

    public CommonRoomRequestService(
        ICommonRoomRequestRepository requestRepository,
        CommonRoomRequestApprovalService approvalService)
    {
        _requestRepository = requestRepository;
        _approvalService = approvalService;
    }

    public List<CommonRoomRequestDto> GetRequestsByCommonRoom(long commonRoomId)
    {
        return _requestRepository.GetAllByCommonRoom(commonRoomId).ToDtoList();
    }

    public CommonRoomRequestDto? GetById(long requestId)
    {
        return _requestRepository.GetById(requestId)?.ToDto();
    }

    public List<DateTime> GetFreeDaysInRange(long requestId)
    {
        return _approvalService.GetFreeDaysInRange(requestId);
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(long requestId)
    {
        return _approvalService.FindAlternativeRanges(requestId);
    }

    public void ApproveWithDate(long requestId, DateTime selectedDate)
    {
        _approvalService.ApproveWithDate(requestId, selectedDate);
    }

    public void RejectRequest(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null)
            return;

        request.Reject();
        _requestRepository.Update(request);
    }

    public void ProposeAlternative(long requestId, int alternativeIndex)
    {
        _approvalService.ProposeAlternative(requestId, alternativeIndex);
    }

    public List<CommonRoomRequestDto> GetByTenantAndBuilding(long tenantId, long buildingId)
    {
        return _requestRepository.GetByTenantAndBuilding(tenantId, buildingId).ToDtoList();
    }

    public void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
        string? validationError = CommonRoomRequest.ValidateDateRange(dateFrom, dateTo);
        if (validationError != null)
            throw new InvalidOperationException(validationError);

        long requestId = _requestRepository.Create(commonRoomId, tenantId, dateFrom, dateTo);
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
            return;

        if (request.CommonRoom.RentalType == RentalType.PerDay)
        {
            _approvalService.GetFreeDaysInRange(request.Id);
            return;
        }

        _approvalService.TryAutoApproveMultiDay(request);
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

        string? validationError = CommonRoomRequest.ValidateDateRange(
            request.ProposedDateFrom!.Value,
            request.ProposedDateTo!.Value);

        if (validationError != null)
            throw new InvalidOperationException(validationError);

        request.AcceptProposedDates();
        _requestRepository.Update(request);

        _approvalService.TryAutoApproveMultiDay(request);
    }
}