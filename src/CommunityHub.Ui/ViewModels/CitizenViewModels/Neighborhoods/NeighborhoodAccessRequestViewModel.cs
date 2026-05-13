using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using System.Windows;
using System.Windows.Media;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class NeighborhoodAccessRequestViewModel : BaseViewModel
{
    private readonly NeighborhoodAccessRequest _request;

    public NeighborhoodAccessRequestViewModel(NeighborhoodAccessRequest request)
    {
        _request = request;
    }

    public long Id => _request.Id;
    public string NeighborhoodName => _request.Neighborhood?.Name ?? string.Empty;
    public string CreatedAtFormatted => _request.CreatedAt.ToString("dd/MM/yyyy");
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

    public bool CanBeDeleted => _request.Status == RequestStatus.PendingApproval;
    public Visibility DeleteVisibility => CanBeDeleted ? Visibility.Visible : Visibility.Collapsed;

    public bool HasRejectionReason => !string.IsNullOrWhiteSpace(_request.RejectionReason);
    public Visibility RejectionVisibility => HasRejectionReason ? Visibility.Visible : Visibility.Collapsed;
    public string RejectionReason => _request.RejectionReason ?? string.Empty;

    public string ImagePath
    {
        get
        {
            var firstImage = _request.Neighborhood?.Images?.FirstOrDefault();
            return firstImage?.Path ?? string.Empty;
        }
    }

    public Visibility NoImageVisibility => string.IsNullOrWhiteSpace(ImagePath)
        ? Visibility.Visible : Visibility.Collapsed;
}