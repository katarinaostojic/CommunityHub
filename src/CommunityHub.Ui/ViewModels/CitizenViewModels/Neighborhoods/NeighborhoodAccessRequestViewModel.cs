using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using System.Windows;
using System.Windows.Media;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class NeighborhoodAccessRequestViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequestDto _request;

    public NeighborhoodAccessRequestViewModel(NeighborhoodAccessRequestDto request)
    {
        _request = request;
    }

    public long Id => _request.Id;
    public string NeighborhoodName => _request.NeighborhoodName;
    public string CreatedAtFormatted => _request.CreatedAtFormatted;
    public RequestStatus Status => _request.Status;

    public string StatusDisplay => _request.Status switch
    {
        RequestStatus.PendingApproval => "Pending approval",
        RequestStatus.Approved => "Approved",
        RequestStatus.Rejected => "Rejected",
        _ => "Unknown"
    };

    public Brush StatusBrush => _request.Status switch
    {
        RequestStatus.PendingApproval => Brushes.Gold,
        RequestStatus.Approved => Brushes.LimeGreen,
        RequestStatus.Rejected => Brushes.Red,
        _ => Brushes.Gray
    };

    public bool CanBeDeleted => _request.CanBeDeleted;
    public Visibility DeleteVisibility => CanBeDeleted ? Visibility.Visible : Visibility.Collapsed;

    public bool HasRejectionReason => _request.HasRejectionReason;
    public Visibility RejectionVisibility => HasRejectionReason ? Visibility.Visible : Visibility.Collapsed;
    public string RejectionReason => _request.RejectionReason ?? string.Empty;
}