using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

public class BuildingAccessRequestViewModel : BaseViewModel
{
    private readonly BuildingAccessRequest _request;

    public BuildingAccessRequestViewModel(BuildingAccessRequest request)
    {
        _request = request;
    }

    public long Id => _request.Id;
    public Building Building => _request.Building;
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

    public bool CancelButtonVisible => _request.Status == RequestStatus.PendingApproval;

    public bool RejectionReasonVisible => _request.Status == RequestStatus.Rejected
                                       && _request.RejectionReason != null;
}