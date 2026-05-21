using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class Neighborhood
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Location Location { get; private set; }
    public decimal Budget { get; private set; }
    public long CoordinatorId { get; private set; }
    public List<Street> Streets { get; private set; }
    public List<Image> Images { get; private set; }

    public Neighborhood(long id, string name, string description, Location location, decimal budget, long coordinatorId)
    {
        Id = id;
        Name = name;
        Description = description;
        Location = location;
        Budget = budget;
        CoordinatorId = coordinatorId;
        Streets = new List<Street>();
        Images = new List<Image>();
    }

    public void AddStreet(Street street) => Streets.Add(street);
    public void AddImage(Image image) => Images.Add(image);

    public bool MatchesSearch(string search)
    {
        if (string.IsNullOrWhiteSpace(search)) return true;
        string lowered = search.ToLower().Trim();
        (string streetPart, int? number) = ParseSearchInput(lowered);
        return MatchesBasicFields(lowered) || MatchesStreet(lowered, streetPart, number);
    }

    public bool ContainsAddress(string fullAddress)
    {
        if (string.IsNullOrWhiteSpace(fullAddress) || !Streets.Any()) return false;
        if (!AddressParser.TryParse(fullAddress, out string userStreet, out int userNumber)) return false;
        return Streets.Any(s =>
            AddressParser.Normalize(s.StreetName) == userStreet &&
            userNumber >= s.StartNumber &&
            userNumber <= s.EndNumber);
    }

    private bool MatchesBasicFields(string search)
    {
        return Name.ToLower().Contains(search) ||
               Location.CityName.ToLower().Contains(search) ||
               Location.CountryName.ToLower().Contains(search);
    }

    private bool MatchesStreet(string search, string streetPart, int? number)
    {
        return Streets.Any(s =>
            s.StreetName.ToLower().Contains(search) ||
            (s.StreetName.ToLower().Contains(streetPart) &&
            (!number.HasValue || (number.Value >= s.StartNumber && number.Value <= s.EndNumber)))
        );
    }

    private (string streetPart, int? number) ParseSearchInput(string search)
    {
        string[] parts = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1 && int.TryParse(parts[^1], out int parsedNumber))
            return (string.Join(" ", parts.Take(parts.Length - 1)), parsedNumber);
        return (search, null);
    }
}