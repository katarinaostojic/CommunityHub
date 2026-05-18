using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces.CommonRoomRepositoryInterfaces;
using CommunityHub.Application.Domain.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Mappings.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Buildings.CommonRooms;

public class CommonRoomRequestService
{
    private readonly ICommonRoomRequestRepository _requestRepository;
    private readonly ICommonRoomRepository _commonRoomRepository;

    public CommonRoomRequestService(
        ICommonRoomRequestRepository requestRepository,
        ICommonRoomRepository commonRoomRepository)
    {
        _requestRepository = requestRepository;
        _commonRoomRepository = commonRoomRepository;
    }

    public List<CommonRoomRequestDto> GetRequestsByCommonRoom(long commonRoomId)
    {
        return _requestRepository.GetAllByCommonRoom(commonRoomId).ToDtoList();
    }

    public CommonRoomRequestDto? GetById(long requestId)
    {
        return _requestRepository.GetById(requestId)?.ToDto();
    }

    public CommonRoomRequestDto? GetDtoById(long requestId)
    {
        return _requestRepository.GetById(requestId)?.ToDto();
    }

    public List<DateTime> GetFreeDaysInRange(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null) return new List<DateTime>();

        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        List<DateTime> freeDays = new List<DateTime>();

        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
        {
            if (request.CommonRoom.IsFreeOnDate(date))
                freeDays.Add(date);
        }

        if (freeDays.Count == 0)
        {
            request.Reject();
            _requestRepository.Update(request);
        }

        return freeDays;
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null) return new List<(DateTime, DateTime)>();

        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        int requestedDays = (int)(request.DateTo - request.DateFrom).TotalDays + 1;
        List<(DateTime, DateTime)> alternatives = new();

        DateTime searchStart = DateTime.Today;
        DateTime searchEnd = request.DateTo.AddDays(30);

        for (DateTime start = searchStart; start <= searchEnd; start = start.AddDays(1))
        {
            DateTime end = start.AddDays(requestedDays - 1);

            if (start == request.DateFrom)
                continue;

            if (IsRangeFree(start, end, request.CommonRoom))
                alternatives.Add((start, end));

            if (alternatives.Count >= 5)
                break;
        }

        return alternatives;
    }

    private bool IsRangeFree(DateTime dateFrom, DateTime dateTo, CommonRoom commonRoom)
    {
        for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            if (!commonRoom.IsFreeOnDate(date))
                return false;
        }

        return true;
    }

    public void ApproveWithDate(long requestId, DateTime selectedDate)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null) return;

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

    private void ApproveMultiDay(CommonRoomRequest request)
    {
        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        if (IsRangeFree(request.DateFrom, request.DateTo, request.CommonRoom))
        {
            ApproveAndBookRange(request);
            return;
        }

        List<(DateTime, DateTime)> alternatives = FindAlternativeRanges(request.Id);

        if (alternatives.Count == 0)
            return;

        var (newFrom, newTo) = alternatives[0];
        request.ProposeNewDateRange(newFrom, newTo);
        _requestRepository.Update(request);
    }

    public void RejectRequest(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null) return;
        request.Reject();
        _requestRepository.Update(request);
    }

    private void UpdateRequest(CommonRoomRequest request)
    {
        _requestRepository.Update(request);
    }

    public void ProposeAlternative(long requestId, int alternativeIndex)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null) return;

        List<(DateTime, DateTime)> alternatives = FindAlternativeRanges(requestId);
        var (newFrom, newTo) = alternatives[alternativeIndex];
        request.ProposeNewDateRange(newFrom, newTo);
        _requestRepository.Update(request);
    }

    public List<CommonRoomRequestDto> GetByTenant(long tenantId)
    {
        return _requestRepository.GetByTenant(tenantId).ToDtoList();
    }

    public List<CommonRoomRequestDto> GetByTenantAndBuilding(long tenantId, long buildingId)
    {
        return _requestRepository.GetByTenantAndBuilding(tenantId, buildingId).ToDtoList();
    }

    public void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
        ValidateRequestedDateRange(dateFrom, dateTo);

        long requestId = _requestRepository.Create(commonRoomId, tenantId, dateFrom, dateTo);

        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null)
            return;

        if (request.CommonRoom.RentalType == RentalType.PerDay)
        {
            GetFreeDaysInRange(request.Id);
            return;
        }

        TryAutoApproveMultiDay(request);
    }

    private static void ValidateRequestedDateRange(DateTime dateFrom, DateTime dateTo)
    {
        if (dateFrom.Date < DateTime.Today)
            throw new InvalidOperationException("Start date cannot be in the past.");

        if (dateTo.Date < DateTime.Today)
            throw new InvalidOperationException("End date cannot be in the past.");

        if (dateTo.Date < dateFrom.Date)
            throw new InvalidOperationException("End date must be after start date.");
    }

    private void TryAutoApproveMultiDay(CommonRoomRequest request)
    {
        if (request.CommonRoom.RentalType != RentalType.MultiDay)
            return;

        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        if (!IsRangeFree(request.DateFrom, request.DateTo, request.CommonRoom))
            return;

        ApproveAndBookRange(request);
    }

    private void ApproveAndBookRange(CommonRoomRequest request)
    {
        request.AutoApprove();
        _requestRepository.Update(request);

        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
            _commonRoomRepository.BookDate(request.CommonRoom.Id, date);
    }

    public void CancelRequest(CommonRoomRequestDto request)
    {
        if (request.Status != CommonRoomRequestStatus.Pending &&
            request.Status != CommonRoomRequestStatus.PendingDateChange)
        {
            throw new InvalidOperationException("Only pending requests can be cancelled.");
        }

        _requestRepository.Delete(request.Id);
    }

    public void AcceptProposedDateChange(CommonRoomRequestDto requestDto)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestDto.Id);

        if (request == null)
            return;

        if (request.Status != CommonRoomRequestStatus.PendingDateChange)
            return;

        if (request.ProposedDateFrom == null || request.ProposedDateTo == null)
            return;

        ValidateRequestedDateRange(request.ProposedDateFrom.Value, request.ProposedDateTo.Value);

        request.AcceptProposedDates();
        _requestRepository.Update(request);

        TryAutoApproveMultiDay(request);
    }
}