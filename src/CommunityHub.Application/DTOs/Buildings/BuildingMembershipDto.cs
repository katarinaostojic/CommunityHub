namespace CommunityHub.Application.DTOs.Buildings;

public class BuildingMembershipDto
{
    public long Id { get; init; }
    public long BuildingId { get; init; }
    public string BuildingStreet { get; init; }
    public string BuildingStreetNumber { get; init; }
    public string BuildingNeighborhood { get; init; }
    public string BuildingCityName { get; init; }
    public string BuildingCountryName { get; init; }
    public string UnitNumber { get; init; }
    public int FloorNumber { get; init; }
    public DateTime ApprovedAt { get; init; }

    public BuildingMembershipDto(
        long id,
        long buildingId,
        string buildingStreet,
        string buildingStreetNumber,
        string buildingNeighborhood,
        string buildingCityName,
        string buildingCountryName,
        string unitNumber,
        int floorNumber,
        DateTime approvedAt)
    {
        Id = id;
        BuildingId = buildingId;
        BuildingStreet = buildingStreet;
        BuildingStreetNumber = buildingStreetNumber;
        BuildingNeighborhood = buildingNeighborhood;
        BuildingCityName = buildingCityName;
        BuildingCountryName = buildingCountryName;
        UnitNumber = unitNumber;
        FloorNumber = floorNumber;
        ApprovedAt = approvedAt;
    }

    public string BuildingFullAddress => $"{BuildingStreet} {BuildingStreetNumber}";
    public string BuildingSubtitle => $"Building: {BuildingStreet} {BuildingStreetNumber}, {BuildingNeighborhood}";
    public string BuildingLocation => $"{BuildingCityName}, {BuildingCountryName}";
}
