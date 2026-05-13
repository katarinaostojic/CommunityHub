using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels;

public class NeighborhoodAccessRequestCoordinatorViewModel
{
    private readonly NeighborhoodAccessRequest _request;

    public NeighborhoodAccessRequestCoordinatorViewModel(NeighborhoodAccessRequest request)
    {
        _request = request;
    }

    public NeighborhoodAccessRequest Request => _request;
    public long Id => _request.Id;
    public User Citizen => _request.Citizen;
    public Neighborhood Neighborhood => _request.Neighborhood;
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

    public bool ApproveRejectVisible => _request.Status == RequestStatus.PendingApproval;
    public bool RejectionReasonVisible => _request.Status == RequestStatus.Rejected
                                       && _request.RejectionReason != null;
}