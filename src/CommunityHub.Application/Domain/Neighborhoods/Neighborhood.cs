namespace CommunityHub.Application.Domain.Neighborhoods;

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

    public bool ContainsAddress(string fullAddress)
    {
        if (string.IsNullOrWhiteSpace(fullAddress)) return false;
        if (!Streets.Any()) return false;
        if (!TryParseAddress(fullAddress, out string userStreet, out int userNumber)) return false;
        return Streets.Any(s =>
            Normalize(s.StreetName) == userStreet &&
            userNumber >= s.StartNumber &&
            userNumber <= s.EndNumber);
    }

    private bool TryParseAddress(string fullAddress, out string streetName, out int streetNumber)
    {
        streetName = string.Empty;
        streetNumber = 0;
        string normalized = Normalize(fullAddress);
        int firstDigitIndex = FindFirstDigitIndex(normalized);
        if (firstDigitIndex == -1) return false;
        string streetPart = normalized[..firstDigitIndex].Trim().Trim(',', '.', '-', '/');
        string numberPart = new string(normalized[firstDigitIndex..].TakeWhile(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(streetPart)) return false;
        if (!int.TryParse(numberPart, out streetNumber)) return false;
        streetName = streetPart;
        return true;
    }

    private int FindFirstDigitIndex(string value)
    {
        for (int i = 0; i < value.Length; i++)
            if (char.IsDigit(value[i]))
                return i;
        return -1;
    }

    private string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        string result = value.Trim().ToLowerInvariant();
        result = result.Replace("š", "s").Replace("đ", "d")
                       .Replace("č", "c").Replace("ć", "c").Replace("ž", "z");
        result = result.Replace("ulica", " ").Replace("ul.", " ").Replace("ul ", " ");
        var sb = new System.Text.StringBuilder();
        foreach (char c in result)
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                sb.Append(c);
        result = sb.ToString();
        while (result.Contains("  "))
            result = result.Replace("  ", " ");
        return result.Trim();
    }
}