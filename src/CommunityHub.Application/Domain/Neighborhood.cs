namespace CommunityHub.Application.Domain;

public class Neighborhood
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Location Location { get; private set; }
    public decimal Budget { get; private set; }
    public long CoordinatorId { get; private set; }
    public List<Street> Streets { get; private set; }
    public List<AppImage> Images { get; private set; }

    public Neighborhood(long id, string name, string description, Location location, decimal budget, long coordinatorId)
    {
        Id = id;
        Name = name;
        Description = description;
        Location = location;
        Budget = budget;
        CoordinatorId = coordinatorId;
        Streets = new List<Street>();
        Images = new List<AppImage>();
    }

    public void AddStreet(Street street)
    {
        Streets.Add(street);
    }

    public void AddImage(AppImage image)
    {
        Images.Add(image);
    }
}