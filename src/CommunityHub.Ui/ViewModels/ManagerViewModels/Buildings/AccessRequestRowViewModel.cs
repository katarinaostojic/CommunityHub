using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;

public class AccessRequestRowViewModel
{
    private readonly BuildingAccessRequestDto _dto;

    public AccessRequestRowViewModel(BuildingAccessRequestDto dto, bool alternateRow)
    {
        _dto = dto;
        AlternateRow = alternateRow;
    }

    public long Id => _dto.Id;
    public string TenantFullName => $"{_dto.TenantFullName}";
    public string BuildingFullAddress => _dto.BuildingFullAddress;
    public string BuildingNeighborhood => _dto.BuildingNeighborhood;
    public string BuildingLocation => _dto.BuildingLocation;
    public string UnitNumber => _dto.UnitNumber;
    public DateTime CreatedAt => _dto.CreatedAt;
    public string? RejectionReason => _dto.RejectionReason;
    public bool AlternateRow { get; }

    public string StatusDisplay => _dto.Status switch
    {
        RequestStatus.PendingApproval => "Pending",
        RequestStatus.Approved => "Approved",
        RequestStatus.Rejected => "Rejected",
        _ => _dto.Status.ToString()
    };

    public string StatusBackground => _dto.Status switch
    {
        RequestStatus.PendingApproval => "#FFF3CD",
        RequestStatus.Approved => "#D4EDDA",
        RequestStatus.Rejected => "#F8D7DA",
        _ => "#F0F0F0"
    };

    public string StatusForeground => _dto.Status switch
    {
        RequestStatus.PendingApproval => "#856404",
        RequestStatus.Approved => "#155724",
        RequestStatus.Rejected => "#721C24",
        _ => "#2C3E50"
    };

    public bool PendingVisible => _dto.Status == RequestStatus.PendingApproval;
    public bool ExplanationVisible => _dto.HasRejectionReason;
}