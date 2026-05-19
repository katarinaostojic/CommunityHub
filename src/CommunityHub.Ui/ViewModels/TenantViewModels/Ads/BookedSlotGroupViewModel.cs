using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class BookedSlotGroupViewModel : BaseViewModel
{
    public BookedSlotGroupViewModel(List<AdSlotDto> slots, AdDto? bookedByAd)
    {
        TenantName = bookedByAd?.AuthorName ?? "Unknown";
        TheirAdDescription = bookedByAd?.Description ?? string.Empty;
        TheirAdType = bookedByAd?.Type.ToDisplayString() ?? string.Empty;
        TheirAdCategory = bookedByAd != null ? bookedByAd.Category.ToDisplayString() : string.Empty;
        TheirAdDateRange = bookedByAd != null
            ? $"{bookedByAd.DateFrom:dd.MM.} – {bookedByAd.DateTo:dd.MM.yyyy}"
            : string.Empty;

        DayGroups = slots
            .GroupBy(s => s.Date)
            .OrderBy(g => g.Key)
            .Select(g => new BookedSlotDayGroupViewModel(g.Key, g.ToList()))
            .ToList();
    }

    public string TenantName { get; }
    public List<BookedSlotDayGroupViewModel> DayGroups { get; }
    public string TheirAdDescription { get; }
    public string TheirAdType { get; }
    public string TheirAdCategory { get; }
    public string TheirAdDateRange { get; }
}