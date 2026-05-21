using CommunityHub.Application.Domain.Shared;
using CommunityHub.Application.DTOs.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BuildingAccessRequestViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestDto _request;

    public BuildingAccessRequestViewModel(BuildingAccessRequestDto request)
    {
        _request = request;
    }

    public long Id => _request.Id;
    public string BuildingAddress => _request.BuildingFullAddress;
    public string BuildingNeighborhood => _request.BuildingNeighborhood;
    public string BuildingLocation => _request.BuildingLocation;
    public string UnitNumber => _request.UnitNumber;
    public DateTime CreatedAt => _request.CreatedAt;
    public RequestStatus Status => _request.Status;

    public string StatusDisplay => _request.Status switch
    {
        RequestStatus.PendingApproval => "⏳ Pending approval",
        RequestStatus.Approved => "✔ Approved",
        RequestStatus.Rejected => "✕ Rejected",
        _ => _request.Status.ToString()
    };

    public string RejectionReasonDisplay => _request.RejectionReason != null
        ? $"Note: {_request.RejectionReason}"
        : string.Empty;

    public bool CanCancel => _request.CanBeCancelled;

    public bool RejectionReasonVisible => _request.HasRejectionReason;
}