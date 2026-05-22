using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Entities.Buildings.CommonRooms;

public class CommonRoomRequestAvailabilityService
{
    private const int MaximumAlternativeCount = 5;
    private const int AlternativeSearchDays = 30;

    private readonly ICommonRoomRepository _commonRoomRepository;

    public CommonRoomRequestAvailabilityService(ICommonRoomRepository commonRoomRepository)
    {
        _commonRoomRepository = commonRoomRepository;
    }

    public List<DateTime> GetFreeDays(CommonRoomRequest request)
    {
        LoadOccupiedDates(request.CommonRoom);

        List<DateTime> freeDays = new();

        for (DateTime date = request.DateFrom; date <= request.DateTo; date = date.AddDays(1))
            AddIfFree(request.CommonRoom, date, freeDays);

        return freeDays;
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(CommonRoomRequest request)
    {
        LoadOccupiedDates(request.CommonRoom);

        DateTime searchEnd = request.DateTo.AddDays(AlternativeSearchDays);

        return FindRanges(request, searchEnd);
    }

    public bool IsRangeFree(DateTime dateFrom, DateTime dateTo, CommonRoom commonRoom)
    {
        LoadOccupiedDates(commonRoom);

        for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            if (!commonRoom.IsFreeOnDate(date))
                return false;
        }

        return true;
    }

    private void LoadOccupiedDates(CommonRoom room)
    {
        List<DateTime> occupiedDates = _commonRoomRepository.GetOccupiedDates(room.Id);
        room.SetOccupiedDates(occupiedDates);
    }

    private static void AddIfFree(CommonRoom room, DateTime date, List<DateTime> freeDays)
    {
        if (room.IsFreeOnDate(date))
            freeDays.Add(date);
    }

    private static List<(DateTime, DateTime)> FindRanges(
        CommonRoomRequest request,
        DateTime searchEnd)
    {
        List<(DateTime, DateTime)> alternatives = new();

        for (DateTime start = DateTime.Today; start <= searchEnd; start = start.AddDays(1))
        {
            if (alternatives.Count >= MaximumAlternativeCount)
                break;

            TryAddRange(request, start, alternatives);
        }

        return alternatives;
    }

    private static void TryAddRange(
        CommonRoomRequest request,
        DateTime start,
        List<(DateTime, DateTime)> alternatives)
    {
        if (start == request.DateFrom)
            return;

        DateTime end = start.AddDays(request.RequestedDays - 1);

        if (IsRangeFreeWithoutReload(start, end, request.CommonRoom))
            alternatives.Add((start, end));
    }

    private static bool IsRangeFreeWithoutReload(DateTime dateFrom, DateTime dateTo, CommonRoom room)
    {
        for (DateTime date = dateFrom; date <= dateTo; date = date.AddDays(1))
        {
            if (!room.IsFreeOnDate(date))
                return false;
        }

        return true;
    }
}