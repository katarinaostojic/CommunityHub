using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Ads;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Ads;

public static class AdSlotMapper
{
    public static (AdSlot slot, Ad? bookedByAd) MapBookedSlotWithAd(IDataReader reader)
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