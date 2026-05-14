namespace CommunityHub.Application.DTOs.TenantAds;

public class BookedAdSlotDto
{
    public BookedAdSlotDto(AdSlotDto slot, AdDto? bookedByAd)
    {
        Slot = slot;
        BookedByAd = bookedByAd;
    }

    public AdSlotDto Slot { get; init; }
    public AdDto? BookedByAd { get; init; }
}