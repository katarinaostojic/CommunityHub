using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.DTOs.Buildings;

public class BuildingAccessRequestDto
{
    public long Id { get; init; }
    public long BuildingId { get; init; }
    public string BuildingStreet { get; init; }
    public string BuildingStreetNumber { get; init; }
    public string BuildingNeighborhood { get; init; }
    public string BuildingCityName { get; init; }
    public string BuildingCountryName { get; init; }
    public string UnitNumber { get; init; }
    public DateTime CreatedAt { get; init; }
    public RequestStatus Status { get; init; }
    public string? RejectionReason { get; init; }
    public string TenantFullName { get; init; }

    public BuildingAccessRequestDto(
        long id,
        long buildingId,
        string buildingStreet,
        string buildingStreetNumber,
        string buildingNeighborhood,
        string buildingCityName,
        string buildingCountryName,
        string unitNumber,
        string tenantFullName,
        DateTime createdAt,
        RequestStatus status,
        string? rejectionReason = null)
    {
        Id = id;
        BuildingId = buildingId;
        BuildingStreet = buildingStreet;
        BuildingStreetNumber = buildingStreetNumber;
        BuildingNeighborhood = buildingNeighborhood;
        BuildingCityName = buildingCityName;
        BuildingCountryName = buildingCountryName;
        UnitNumber = unitNumber;
        TenantFullName = tenantFullName;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public string BuildingFullAddress => $"{BuildingStreet} {BuildingStreetNumber}";
    public string BuildingLocation => $"{BuildingCityName}, {BuildingCountryName}";
    public bool CanBeCancelled => Status == RequestStatus.PendingApproval;
    public bool HasRejectionReason => Status == RequestStatus.Rejected && RejectionReason != null;
}