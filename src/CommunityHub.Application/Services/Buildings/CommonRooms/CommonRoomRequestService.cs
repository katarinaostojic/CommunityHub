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

        LoadOccupiedDates(request.CommonRoom);
        List<DateTime> freeDays = CollectFreeDays(request);

        if (freeDays.Count == 0)
            RejectAndSave(request);

        return freeDays;
    }

    private void LoadOccupiedDates(CommonRoom room)
    {
        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(room.Id);
        room.SetOccupiedDates(occupied);
    }

    private static List<DateTime> CollectFreeDays(CommonRoomRequest request)
    {
        List<DateTime> freeDays = new List<DateTime>();
        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
        {
            if (request.CommonRoom.IsFreeOnDate(date))
                freeDays.Add(date);
        }
        return freeDays;
    }

    private void RejectAndSave(CommonRoomRequest request)
    {
        request.Reject();
        _requestRepository.Update(request);
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(long requestId)
    {
        CommonRoomRequest? request = _requestRepository.GetById(requestId);
        if (request == null) return new List<(DateTime, DateTime)>();

        LoadOccupiedDates(request.CommonRoom);
        return SearchAlternativeRanges(request);
    }

    private List<(DateTime, DateTime)> SearchAlternativeRanges(CommonRoomRequest request)
    {
        int requestedDays = (int)(request.DateTo - request.DateFrom).TotalDays + 1;
        List<(DateTime, DateTime)> alternatives = new();

        DateTime searchStart = DateTime.Today;
        DateTime searchEnd = request.DateTo.AddDays(30);

        for (DateTime start = searchStart; start <= searchEnd; start = start.AddDays(1))
        {
            if (alternatives.Count >= 5) break;
            TryAddAlternative(start, requestedDays, request, alternatives);
        }

        return alternatives;
    }

    private void TryAddAlternative(DateTime start, int requestedDays, CommonRoomRequest request, List<(DateTime, DateTime)> alternatives)
    {
        if (start == request.DateFrom) return;

        DateTime end = start.AddDays(requestedDays - 1);
        if (IsRangeFree(start, end, request.CommonRoom))
            alternatives.Add((start, end));
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