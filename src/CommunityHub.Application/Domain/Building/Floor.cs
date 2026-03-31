namespace CommunityHub.Application.Domain.Building;

public class Floor
{
    public long Id { get; private set; }
    public Building Building { get; private set; }
    public int FloorNumber { get; private set; }
    public List<Unit> Units { get; private set; }

    public Floor(long id, Building building, int floorNumber)
    {
        Id = id;
        Building = building;
        FloorNumber = floorNumber;
        Units = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        Units.Add(unit);
    }
}