using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.TenantAds;

namespace CommunityHub.Application.Mappings.TenantAds;

public static class AdMappingExtensions
{
    public static AdDto ToTenantAdDto(this Ad ad)
    {
        return new AdDto(
            id: ad.Id,
            buildingId: ad.BuildingId,
            authorId: ad.Author.Id,
            authorName: ad.Author.DisplayName,
            type: ad.Type,
            category: ad.Category,
            description: ad.Description,
            dateFrom: ad.DateFrom,
            dateTo: ad.DateTo,
            status: ad.Status,
            slots: ad.Slots.ToTenantAdSlotDtoList()
        );
    }

    public static List<AdDto> ToTenantAdDtoList(this IEnumerable<Ad> ads)
    {
        return ads.Select(ad => ad.ToTenantAdDto()).ToList();
    }

    public static AdSlotDto ToTenantAdSlotDto(this AdSlot slot)
    {
        return new AdSlotDto(
            id: slot.Id,
            adId: slot.AdId,
            date: slot.Date,
            startTime: slot.StartTime,
            endTime: slot.EndTime,
            bookedByAdId: slot.BookedByAdId
        );
    }

    public static List<AdSlotDto> ToTenantAdSlotDtoList(this IEnumerable<AdSlot> slots)
    {
        return slots.Select(slot => slot.ToTenantAdSlotDto()).ToList();
    }

    public static AdNotificationDto ToTenantAdNotificationDto(this AdNotification notification)
    {
        return new AdNotificationDto(
            id: notification.Id,
            recipientId: notification.RecipientId,
            ad: notification.Ad.ToTenantAdDto(),
            bookedByAd: notification.BookedByAd.ToTenantAdDto(),
            createdAt: notification.CreatedAt,
            isRead: notification.IsRead
        );
    }

    public static List<AdNotificationDto> ToTenantAdNotificationDtoList(this IEnumerable<AdNotification> notifications)
    {
        return notifications.Select(notification => notification.ToTenantAdNotificationDto()).ToList();
    }

    public static BookedAdSlotDto ToTenantBookedAdSlotDto(this (AdSlot slot, Ad? bookedByAd) bookedSlot)
    {
        return new BookedAdSlotDto(
            slot: bookedSlot.slot.ToTenantAdSlotDto(),
            bookedByAd: bookedSlot.bookedByAd?.ToTenantAdDto()
        );
    }

    public static List<BookedAdSlotDto> ToTenantBookedAdSlotDtoList(this IEnumerable<(AdSlot slot, Ad? bookedByAd)> bookedSlots)
    {
        return bookedSlots.Select(bookedSlot => bookedSlot.ToTenantBookedAdSlotDto()).ToList();
    }
}