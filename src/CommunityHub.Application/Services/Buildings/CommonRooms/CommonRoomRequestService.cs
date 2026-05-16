using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces.CommonRoomRepositoryInterfaces;
using CommunityHub.Application.Domain.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Mappings.Buildings;
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

    public CommonRoomRequest? GetById(long requestId)
    {
        return _requestRepository.GetById(requestId);
    }

    public List<DateTime> GetFreeDaysInRange(CommonRoomRequest request)
    {
        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        List<DateTime> freeDays = new List<DateTime>();
        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
        {
            if (request.CommonRoom.IsFreeOnDate(date))
                freeDays.Add(date);
        }
        return freeDays;
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(CommonRoomRequest request)
    {
        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        int requestedDays = (int)(request.DateTo - request.DateFrom).TotalDays + 1;
        List<(DateTime, DateTime)> alternatives = new List<(DateTime, DateTime)>();
        DateTime searchStart = DateTime.Today;
        DateTime searchEnd = request.DateTo.AddDays(30);

        for (DateTime start = searchStart; start <= searchEnd; start = start.AddDays(1))
        {
            DateTime end = start.AddDays(requestedDays - 1);
            if (start == request.DateFrom) continue;
            if (IsRangeFree(start, end, request.CommonRoom))
                alternatives.Add((start, end));
            if (alternatives.Count >= 5) break;
        }
        return alternatives;
    }

    public bool IsRangeFree(DateTime dateFrom, DateTime dateTo, CommonRoom commonRoom)
    {
        for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            if (!commonRoom.IsFreeOnDate(date))
                return false;
        }
        return true;
    }

    public void ApproveWithDate(CommonRoomRequest request, DateTime selectedDate)
    {
        request.ApproveWithDate(selectedDate);
        _requestRepository.Update(request);
        _commonRoomRepository.BookDate(request.CommonRoom.Id, selectedDate);
    }

    public void ApproveMultiDay(CommonRoomRequest request)
    {
        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        bool isFree = IsRangeFree(request.DateFrom, request.DateTo, request.CommonRoom);
        if (isFree)
        {
            request.AutoApprove();
            _requestRepository.Update(request);
            for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
                _commonRoomRepository.BookDate(request.CommonRoom.Id, date);
        }
        else
        {
            List<(DateTime, DateTime)> alternatives = FindAlternativeRanges(request);
            if (alternatives.Count > 0)
            {
                var (newFrom, newTo) = alternatives[0];
                request.ProposeNewDateRange(newFrom, newTo);
                _requestRepository.Update(request);
            }
        }
    }

    public void RejectRequest(CommonRoomRequest request)
    {
        request.Reject();
        _requestRepository.Update(request);
    }

    public void UpdateRequest(CommonRoomRequest request)
    {
        _requestRepository.Update(request);
    }

    public List<CommonRoomRequestDto> GetByTenant(long tenantId)
    {
        return _requestRepository.GetByTenant(tenantId).ToDtoList();
    }

    public void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
        _requestRepository.Create(commonRoomId, tenantId, dateFrom, dateTo);

        CommonRoomRequest? request = _requestRepository.GetByTenant(tenantId)
            .FirstOrDefault(r => r.CommonRoom.Id == commonRoomId
                              && r.DateFrom.Date == dateFrom.Date
                              && r.DateTo.Date == dateTo.Date
                              && r.Status == CommonRoomRequestStatus.Pending);

        if (request == null) return;

        TryAutoApproveMultiDay(request);
    }

    private void TryAutoApproveMultiDay(CommonRoomRequest request)
    {
        if (request.CommonRoom.RentalType != RentalType.MultiDay) return;

        List<DateTime> occupied = _commonRoomRepository.GetOccupiedDates(request.CommonRoom.Id);
        request.CommonRoom.SetOccupiedDates(occupied);

        if (!IsRangeFree(request.DateFrom, request.DateTo, request.CommonRoom)) return;

        request.AutoApprove();
        _requestRepository.Update(request);

        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
            _commonRoomRepository.BookDate(request.CommonRoom.Id, date);
    }

    public void CancelRequest(CommonRoomRequest request)
    {
        _requestRepository.Delete(request.Id);
    }

    public void AcceptProposedDateChange(CommonRoomRequest request)
    {
        if (request.ProposedDateFrom == null || request.ProposedDateTo == null) return;

        request.AcceptProposedDates();
        request.AutoApprove();
        _requestRepository.Update(request);

        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
            _commonRoomRepository.BookDate(request.CommonRoom.Id, date);
    }
}