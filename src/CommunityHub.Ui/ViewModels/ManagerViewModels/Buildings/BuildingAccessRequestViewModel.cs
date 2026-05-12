using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;

public class BuildingAccessRequestViewModel
{
    private readonly BuildingAccessRequest _request;

    public BuildingAccessRequestViewModel(BuildingAccessRequest request)
    {
        _request = request;
    }

    public BuildingAccessRequest Request => _request;
    public User Tenant => _request.Tenant;
    public Building Building => _request.Building;
    public string UnitNumber => _request.UnitNumber;
    public DateTime CreatedAt => _request.CreatedAt;
    public string? RejectionReason => _request.RejectionReason;

    public string StatusDisplay => _request.Status switch
    {
        RequestStatus.PendingApproval => "Pending",
        RequestStatus.Approved => "Accepted",
        RequestStatus.Rejected => "Rejected",
        _ => _request.Status.ToString()
    };

    public bool PendingVisible => _request.Status == RequestStatus.PendingApproval;
    public bool ExplanationVisible => _request.Status == RequestStatus.Rejected;
}