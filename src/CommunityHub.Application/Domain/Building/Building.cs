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
    public List<string> ImagePaths { get; private set; }

    public Building(long id, string street, string streetNumber, string neighborhood, City city, int numberOfFloors)
    {
        Id = id;
        Street = street;
        StreetNumber = streetNumber;
        Neighborhood = neighborhood;
        City = city;
        NumberOfFloors = numberOfFloors;
        Floors = new List<Floor>();
        ImagePaths = new List<string>();
    }

    public void AddFloor(Floor floor)
    {
        Floors.Add(floor);
    }

    public void AddImagePath(string imagePath)
    {
        ImagePaths.Add(imagePath);
    }

    public int TotalUnits => Floors.Sum(f => f.Units.Count);
}