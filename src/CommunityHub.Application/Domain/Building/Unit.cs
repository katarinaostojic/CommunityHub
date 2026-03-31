namespace CommunityHub.Application.Domain.Building;

public class Unit
{
    public long Id { get; private set; }
    public Floor Floor { get; private set; }
    public string UnitNumber { get; private set; }

    public Unit(long id, Floor floor, string unitNumber)
    {
        Id = id;
        Floor = floor;
        UnitNumber = unitNumber;
    }
}