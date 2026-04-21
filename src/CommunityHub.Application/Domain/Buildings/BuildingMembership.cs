namespace CommunityHub.Application.Domain.Buildings;

public class BuildingMembership
{
    public long Id { get; private set; }
    public Building Building { get; private set; }
    public User User { get; private set; }
    public string UnitNumber { get; private set; }
    public int FloorNumber { get; private set; }
    public DateTime ApprovedAt { get; private set; }

    public BuildingMembership(long id, Building building, User user, string unitNumber, int floorNumber, DateTime approvedAt)
    {
        Id = id;
        Building = building;
        User = user;
        UnitNumber = unitNumber;
        FloorNumber = floorNumber;
        ApprovedAt = approvedAt;
    }
}