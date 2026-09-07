using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Neighborhoods;
using System.Windows;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class NeighborhoodAccessRequestCoordinatorViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequestDto _request;
    private bool _showApproveConfirm = false;
    private bool _showDeclineConfirm = false;
    private RequestStatus _currentStatus;
    private string _successMessage = string.Empty;

    public NeighborhoodAccessRequestCoordinatorViewModel(NeighborhoodAccessRequestDto request)
    {
        _request = request;
        _currentStatus = request.Status;
    }

    public long Id => _request.Id;
    public string CitizenFullName => _request.CitizenFullName;
    public string CitizenAddress => _request.Address;
    public string NeighborhoodName => _request.NeighborhoodName;
    public DateTime CreatedAt => _request.CreatedAt;
    public RequestStatus Status => _currentStatus;

    public string StatusDisplay => _currentStatus switch
    {
        RequestStatus.PendingApproval => "⏳ Pending approval",
        RequestStatus.Approved => "✔ Approved",
        RequestStatus.Rejected => "✕ Rejected",
        _ => _currentStatus.ToString()
    };

    public string RejectionReasonDisplay => _currentStatus == RequestStatus.Rejected && _request.RejectionReason != null
        ? $"Note: {_request.RejectionReason}"
        : string.Empty;

    public bool ApproveRejectVisible => _currentStatus == RequestStatus.PendingApproval;
    public bool RejectionReasonVisible => _currentStatus == RequestStatus.Rejected
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

    public string SuccessMessage
    {
        get => _successMessage;
        set
        {
            SetProperty(ref _successMessage, value);
            OnPropertyChanged(nameof(SuccessMessageVisibility));
        }
    }

    public Visibility SuccessMessageVisibility => string.IsNullOrEmpty(_successMessage)
        ? Visibility.Collapsed
        : Visibility.Visible;

    public void MarkAsApproved()
    {
        _currentStatus = RequestStatus.Approved;
        OnPropertyChanged(nameof(StatusDisplay));
        OnPropertyChanged(nameof(ApproveRejectVisible));
        OnPropertyChanged(nameof(RejectionReasonDisplay));
    }

    public void MarkAsRejected()
    {
        _currentStatus = RequestStatus.Rejected;
        OnPropertyChanged(nameof(StatusDisplay));
        OnPropertyChanged(nameof(ApproveRejectVisible));
        OnPropertyChanged(nameof(RejectionReasonDisplay));
    }
}