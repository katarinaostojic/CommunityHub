using CommunityHub.Application.Domain.Building;
using System.Windows;

namespace CommunityHub.Ui.ViewModels;

public class BuildingAccessRequestViewModel
{
    public long Id { get; set; }
    public Building Building { get; set; }
    public string UnitNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public string? RejectionReason { get; set; }

    public string StatusDisplay => Status switch
    {
        "pending approval" => "⏳ Pending approval",
        "accepted" => "✔ Accepted",
        "rejected" => "✕ Rejected",
        _ => Status
    };

    public string RejectionReasonDisplay => RejectionReason != null
        ? $"Note: {RejectionReason}"
        : string.Empty;

    public Visibility CancelButtonVisibility =>
        Status == "pending approval" ? Visibility.Visible : Visibility.Collapsed;

    public Visibility RejectionReasonVisibility =>
        Status == "rejected" && RejectionReason != null ? Visibility.Visible : Visibility.Collapsed;
}