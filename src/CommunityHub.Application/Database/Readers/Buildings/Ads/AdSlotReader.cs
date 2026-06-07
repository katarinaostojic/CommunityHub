using CommunityHub.Application.Database.Mappers.Buildings.Ads;
using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings.Ads;

public static class AdSlotReader
{
    public static List<AdSlot> ReadSlots(IDataReader reader)
    {
        List<AdSlot> slots = new();

        while (reader.Read())
        {
            slots.Add(AdMapper.MapSlot(reader));
        }

        return slots;
    }

    public static List<(AdSlot slot, Ad? bookedByAd)> ReadBookedSlotsWithAds(IDataReader reader)
    {
        List<(AdSlot slot, Ad? bookedByAd)> results = new();

        while (reader.Read())
        {
            results.Add(AdSlotMapper.MapBookedSlotWithAd(reader));
        }

        return results;
    }

    public static User? ReadSingleUser(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return UserMapper.Map(reader);
    }
}