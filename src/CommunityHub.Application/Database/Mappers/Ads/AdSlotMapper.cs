using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Ads;

public static class AdSlotMapper
{
    public static List<AdSlot> ReadSlots(IDataReader reader)
    {
        List<AdSlot> slots = new();

        while (reader.Read())
            slots.Add(AdMapper.MapSlot(reader));

        return slots;
    }

    public static List<(AdSlot slot, Ad? bookedByAd)> ReadBookedSlotsWithAds(IDataReader reader)
    {
        List<(AdSlot, Ad?)> results = new();

        while (reader.Read())
            results.Add(MapBookedSlotWithAd(reader));

        return results;
    }

    private static (AdSlot slot, Ad? bookedByAd) MapBookedSlotWithAd(IDataReader reader)
    {
        AdSlot slot = AdMapper.MapSlot(reader);
        Ad? bookedByAd = HasBookedByAd(reader)
            ? AdMapper.MapBookedByAd(reader, UserMapper.Map(reader))
            : null;

        return (slot, bookedByAd);
    }

    private static bool HasBookedByAd(IDataReader reader)
    {
        return !reader.IsDBNull(reader.GetOrdinal("ba_id"));
    }
}