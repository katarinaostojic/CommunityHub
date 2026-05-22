using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.DTOs.Ads;

namespace CommunityHub.Application.Mappings.Ads;

public static class AdMappingExtensions
{
    public static AdDto ToAdDto(this Ad ad)
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
            slots: ad.Slots.ToAdSlotDtoList()
        );
    }

    public static List<AdDto> ToAdDtoList(this IEnumerable<Ad> ads)
    {
        return ads.Select(ad => ad.ToAdDto()).ToList();
    }

    public static AdSlotDto ToAdSlotDto(this AdSlot slot)
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

    public static List<AdSlotDto> ToAdSlotDtoList(this IEnumerable<AdSlot> slots)
    {
        return slots.Select(slot => slot.ToAdSlotDto()).ToList();
    }

    public static AdNotificationDto ToAdNotificationDto(this AdNotification notification)
    {
        return new AdNotificationDto(
            id: notification.Id,
            recipientId: notification.RecipientId,
            ad: notification.Ad.ToAdDto(),
            bookedByAd: notification.BookedByAd.ToAdDto(),
            createdAt: notification.CreatedAt,
            isRead: notification.IsRead
        );
    }

    public static List<AdNotificationDto> ToAdNotificationDtoList(this IEnumerable<AdNotification> notifications)
    {
        return notifications.Select(notification => notification.ToAdNotificationDto()).ToList();
    }

    public static BookedAdSlotDto ToBookedAdSlotDto(this (AdSlot slot, Ad? bookedByAd) bookedSlot)
    {
        return new BookedAdSlotDto(
            slot: bookedSlot.slot.ToAdSlotDto(),
            bookedByAd: bookedSlot.bookedByAd?.ToAdDto()
        );
    }

    public static List<BookedAdSlotDto> ToBookedAdSlotDtoList(this IEnumerable<(AdSlot slot, Ad? bookedByAd)> bookedSlots)
    {
        return bookedSlots.Select(bookedSlot => bookedSlot.ToBookedAdSlotDto()).ToList();
    }
}