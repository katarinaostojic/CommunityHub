using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class NeighborhoodAccessRequestCoordinatorViewModel
{
    private readonly NeighborhoodAccessRequestDto _request;

    public NeighborhoodAccessRequestCoordinatorViewModel(NeighborhoodAccessRequestDto request)
    {
        _request = request;
    }

    public long Id => _request.Id;
    public string CitizenFullName => _request.CitizenFullName;
    public string CitizenAddress => _request.Address;
    public string NeighborhoodName => _request.NeighborhoodName;
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