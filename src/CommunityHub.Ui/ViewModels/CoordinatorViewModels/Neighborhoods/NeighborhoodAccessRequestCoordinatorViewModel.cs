using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Neighborhoods;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class NeighborhoodAccessRequestCoordinatorViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequestDto _request;
    private bool _showApproveConfirm = false;
    private bool _showDeclineConfirm = false;

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

    public string RejectionReasonDisplay => _request.Status == RequestStatus.Rejected && _request.RejectionReason != null
        ? $"Note: {_request.RejectionReason}"
        : string.Empty;

    public bool ApproveRejectVisible => _request.Status == RequestStatus.PendingApproval;
    public bool RejectionReasonVisible => _request.Status == RequestStatus.Rejected
                                       && _request.RejectionReason != null;

    public bool ShowApproveConfirm
    {
        get => _showApproveConfirm;
        set
        {
            SetProperty(ref _showApproveConfirm, value);
            OnPropertyChanged(nameof(ApproveConfirmVisibility));
        }
    }

    public bool ShowDeclineConfirm
    {
        get => _showDeclineConfirm;
        set
        {
            SetProperty(ref _showDeclineConfirm, value);
            OnPropertyChanged(nameof(DeclineConfirmVisibility));
        }
    }

    public Visibility ApproveConfirmVisibility => ShowApproveConfirm ? Visibility.Visible : Visibility.Collapsed;
    public Visibility DeclineConfirmVisibility => ShowDeclineConfirm ? Visibility.Visible : Visibility.Collapsed;

    public void OpenApproveConfirm()
    {
        ShowApproveConfirm = true;
        ShowDeclineConfirm = false;
    }

    public void OpenDeclineConfirm()
    {
        ShowDeclineConfirm = true;
        ShowApproveConfirm = false;
    }

    public void CloseConfirm()
    {
        ShowApproveConfirm = false;
        ShowDeclineConfirm = false;
    }
}