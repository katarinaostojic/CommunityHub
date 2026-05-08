using CommunityHub.Application.Domain.Ads;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class BookedSlotGroupViewModel : BaseViewModel
{
    public BookedSlotGroupViewModel(List<AdSlot> slots, Ad? bookedByAd)
    {
        Slots = slots.Select(s => new SlotChipViewModel(s)).ToList();
        TenantName = bookedByAd?.Author.DisplayName ?? "Unknown";
        TheirAdDescription = bookedByAd?.Description ?? string.Empty;
        TheirAdType = bookedByAd?.Type == AdType.Offering ? "↑ Offering" : "↓ Seeking";
        TheirAdCategory = bookedByAd != null ? bookedByAd.Category.ToDisplayString() : string.Empty;
        TheirAdDateRange = bookedByAd != null
            ? $"{bookedByAd.DateFrom:dd.MM.} – {bookedByAd.DateTo:dd.MM.yyyy}"
            : string.Empty;
    }

    public string TenantName { get; }
    public List<SlotChipViewModel> Slots { get; }
    public string TheirAdDescription { get; }
    public string TheirAdType { get; }
    public string TheirAdCategory { get; }
    public string TheirAdDateRange { get; }
}