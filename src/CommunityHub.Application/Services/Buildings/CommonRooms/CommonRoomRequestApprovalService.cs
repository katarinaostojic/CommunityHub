using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Buildings.CommonRooms;

public class CommonRoomRequestApprovalService
{
    private readonly ICommonRoomRequestRepository _requestRepository;
    private readonly ICommonRoomRepository _commonRoomRepository;
    private readonly CommonRoomRequestAvailabilityService _availabilityService;

    public CommonRoomRequestApprovalService(
        ICommonRoomRequestRepository requestRepository,
        ICommonRoomRepository commonRoomRepository,
        CommonRoomRequestAvailabilityService availabilityService)
    {
        _requestRepository = requestRepository;
        _commonRoomRepository = commonRoomRepository;
        _availabilityService = availabilityService;
    }

    public List<DateTime> GetFreeDaysInRange(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
        {
            return new List<DateTime>();
        }

        List<DateTime> freeDays = _availabilityService.GetFreeDays(request);

        if (freeDays.Count == 0)
        {
            RejectAndSave(request);
        }

        return freeDays;
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
        {
            return new List<(DateTime, DateTime)>();
        }

        return _availabilityService.FindAlternativeRanges(request);
    }

    public void ApproveWithDate(long requestId, DateTime selectedDate)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
        {
            return;
        }

        ApproveSingleDate(request, selectedDate);
    }

    public void ProposeAlternative(long requestId, int alternativeIndex)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);

        if (request == null)
        {
            return;
        }

        List<(DateTime, DateTime)> alternatives = _availabilityService.FindAlternativeRanges(request);
        ApplyAlternative(request, alternatives[alternativeIndex]);
    }

    public void TryAutoApproveMultiDay(CommonRoomRequest request)
    {
        if (!CanAutoApprove(request))
        {
            return;
        }

        ApproveAndBookRange(request);
    }

    private void ApproveSingleDate(CommonRoomRequest request, DateTime selectedDate)
    {
        try
        {
            request.ApproveWithDate(selectedDate);
            _requestRepository.Update(request);
            _commonRoomRepository.BookDate(request.CommonRoom.Id, selectedDate);
        }
        catch (Exception)
        {
            throw new InvalidOperationException("This date is already booked. Please select a different day.");
        }
    }

    private void ApplyAlternative(
        CommonRoomRequest request,
        (DateTime DateFrom, DateTime DateTo) alternative)
    {
        request.ProposeNewDateRange(alternative.DateFrom, alternative.DateTo);
        _requestRepository.Update(request);
    }

    private bool CanAutoApprove(CommonRoomRequest request)
    {
        return request.CommonRoom.RentalType == RentalType.MultiDay &&
               _availabilityService.IsRangeFree(request.DateFrom, request.DateTo, request.CommonRoom);
    }

    private void ApproveAndBookRange(CommonRoomRequest request)
    {
        request.AutoApprove();
        _requestRepository.Update(request);

        BookRange(request);
    }

    private void BookRange(CommonRoomRequest request)
    {
        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
        {
            _commonRoomRepository.BookDate(request.CommonRoom.Id, date);
        }
    }

    private void RejectAndSave(CommonRoomRequest request)
    {
        request.Reject();
        _requestRepository.Update(request);
    }
}