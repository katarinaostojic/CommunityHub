namespace CommunityHub.Application.Domain.Entities.Neighborhoods.CityGifts;

public class CityGiftApplication
{
    public long Id { get; private set; }
    public long CityGiftId { get; private set; }
    public long CoordinatorId { get; private set; }
    public long NeighborhoodId { get; private set; }
    public DateOnly AppliedAt { get; private set; }

    public CityGiftApplication(long id, long cityGiftId, long coordinatorId,
        long neighborhoodId, DateOnly appliedAt)
    {
        Id = id;
        CityGiftId = cityGiftId;
        CoordinatorId = coordinatorId;
        NeighborhoodId = neighborhoodId;
        AppliedAt = appliedAt;
    }
}