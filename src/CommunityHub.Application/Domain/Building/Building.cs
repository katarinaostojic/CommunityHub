namespace CommunityHub.Application.Domain.Building;

public class Building
{
    public long Id { get; private set; }
    public string Street { get; private set; }
    public string StreetNumber { get; private set; }
    public string Neighborhood { get; private set; }
    public City City { get; private set; }
    public int NumberOfFloors { get; private set; }
    public List<Floor> Floors { get; private set; }
    public List<Image> Images { get; private set; }

    public List<BuildingMembership> Memberships { get; private set; } = new List<BuildingMembership>();
    public List<BuildingAccessRequest> AccessRequests { get; private set; } = new List<BuildingAccessRequest>();

    public Building(long id, string street, string streetNumber, string neighborhood, City city, int numberOfFloors)
    {
        Id = id;
        Street = street;
        StreetNumber = streetNumber;
        Neighborhood = neighborhood;
        City = city;
        NumberOfFloors = numberOfFloors;
        Floors = new List<Floor>();
        Images = new List<Image>();
    }

    public void AddFloor(Floor floor)
    {
        Floors.Add(floor);
    }

    public void AddImage(Image image)
    {
        Images.Add(image);
    }

    public int TotalUnits => Floors.Sum(f => f.Units.Count);

    public void AddMembership(BuildingMembership membership)
    {
        Memberships.Add(membership);
    }

    public void AddAccessRequest(BuildingAccessRequest request)
    {
        AccessRequests.Add(request);
    }

    public bool IsUnitOccupied(string unitNumber)
    {
        return Memberships.Any(m => m.UnitNumber == unitNumber);
    }

    public bool ContainsUnit(string unitNumber)
    {
        return Floors
            .SelectMany(f => f.Units)
            .Any(u => u.UnitNumber == unitNumber);
    }

    public bool HasExistingRequest(long userId, string unitNumber)
    {
        return AccessRequests.Any(r => r.User.Id == userId
            && r.UnitNumber == unitNumber
            && r.Status == RequestStatus.PendingApproval);
    }
}