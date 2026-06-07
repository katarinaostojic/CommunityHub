using CommunityHub.Application.Database.Mappers.Buildings.Ads;
using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings.Ads;

public static class AdNotificationReader
{
    public static List<AdNotification> ReadNotifications(IDataReader reader)
    {
        List<AdNotification> notifications = new();

        while (reader.Read())
        {
            notifications.Add(AdNotificationMapper.Map(reader));
        }

        return notifications;
    }
}