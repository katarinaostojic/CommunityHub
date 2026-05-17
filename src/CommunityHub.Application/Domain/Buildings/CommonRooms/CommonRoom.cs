namespace CommunityHub.Application.Domain.Buildings.CommonRooms;

public enum RentalType
{
    PerDay,
    MultiDay
}

public class CommonRoom
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int FloorNumber { get; private set; }
    public RentalType RentalType { get; private set; }
    public long BuildingId { get; private set; }

    private List<DateTime> _occupiedDates;

    public CommonRoom(long id, string name, string description, int floorNumber,
                      RentalType rentalType, long buildingId)
    {
        Id = id;
        Name = name;
        Description = description;
        FloorNumber = floorNumber;
        RentalType = rentalType;
        BuildingId = buildingId;
        _occupiedDates = new List<DateTime>();
    }

    public void SetOccupiedDates(List<DateTime> dates)
    {
        _occupiedDates = dates;
    }

    public bool IsFloorValid(int totalFloors)
    {
        return FloorNumber >= 0 && FloorNumber <= totalFloors;
    }

    public bool IsFreeOnDate(DateTime date)
    {
        return !_occupiedDates.Any(d => d.Date == date.Date);
    }

    public List<DateTime> GetOccupiedDates() => _occupiedDates;
}