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

        return request.CommonRoom.GetFreeDatesInRange(
            request.DateFrom,
            request.DateTo);
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

        return commonRoom.IsRangeFree(dateFrom, dateTo);
    }

    private void LoadOccupiedDates(CommonRoom room)
    {
        List<DateTime> occupiedDates = _commonRoomRepository.GetOccupiedDates(room.Id);
        room.SetOccupiedDates(occupiedDates);
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

        if (request.CommonRoom.IsRangeFree(start, end))
            alternatives.Add((start, end));
    }
}