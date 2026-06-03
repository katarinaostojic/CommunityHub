namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class CityObjectReservation
{
    public long Id { get; private set; }
    public long CityObjectId { get; private set; }
    public long NeighborhoodId { get; private set; }
    public DateOnly DateFrom { get; private set; }
    public DateOnly DateTo { get; private set; }

    public CityObjectReservation(long id, long cityObjectId, long neighborhoodId,
        DateOnly dateFrom, DateOnly dateTo)
    {
        Id = id;
        CityObjectId = cityObjectId;
        NeighborhoodId = neighborhoodId;
        DateFrom = dateFrom;
        DateTo = dateTo;
    }

    public bool OverlapsWith(DateOnly from, DateOnly to)
        => DateFrom <= to && DateTo >= from;
}
