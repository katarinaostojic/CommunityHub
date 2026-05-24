using CommunityHub.Application.Domain.Entities.Buildings;
using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Ads;

public static class AdMapper
{
    public static Ad Map(IDataReader reader, User author)
    {
        return new Ad(
            id: Convert.ToInt64(reader["id"]),
            buildingId: Convert.ToInt64(reader["building_id"]),
            author: author,
            type: MapType(reader["type"].ToString()!),
            category: MapCategory(reader["category"].ToString()!),
            description: reader["description"].ToString()!,
            dateFrom: (DateOnly)reader["date_from"],
            dateTo: (DateOnly)reader["date_to"],
            status: MapStatus(reader["status"].ToString()!)
        );
    }

    public static Ad MapBookedByAd(IDataReader reader, User author)
    {
        return new Ad(
            id: Convert.ToInt64(reader["ba_id"]),
            buildingId: Convert.ToInt64(reader["ba_building_id"]),
            author: author,
            type: MapType(reader["ba_type"].ToString()!),
            category: MapCategory(reader["ba_category"].ToString()!),
            description: reader["ba_description"].ToString()!,
            dateFrom: (DateOnly)reader["ba_date_from"],
            dateTo: (DateOnly)reader["ba_date_to"],
            status: MapStatus(reader["ba_status"].ToString()!)
        );
    }

    public static AdSlot MapSlot(IDataReader reader)
    {
        long? bookedByAdId = reader.IsDBNull(reader.GetOrdinal("booked_by_ad_id"))
            ? null
            : Convert.ToInt64(reader["booked_by_ad_id"]);

        return new AdSlot(
            id: Convert.ToInt64(reader["slot_id"]),
            adId: Convert.ToInt64(reader["ad_id"]),
            date: (DateOnly)reader["date"],
            startTime: (TimeOnly)reader["start_time"],
            endTime: (TimeOnly)reader["end_time"],
            bookedByAdId: bookedByAdId
        );
    }

    public static AdType MapType(string value) => value switch
    {
        "offering" => AdType.Offering,
        "seeking" => AdType.Seeking,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    public static string ToDbType(AdType type) => type switch
    {
        AdType.Offering => "offering",
        AdType.Seeking => "seeking",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    public static AdCategory MapCategory(string value) => value switch
    {
        "moving" => AdCategory.Moving,
        "appliance_repair" => AdCategory.ApplianceRepair,
        "lending" => AdCategory.Lending,
        "cleaning" => AdCategory.Cleaning,
        "other" => AdCategory.Other,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    public static string ToDbCategory(AdCategory category) => category switch
    {
        AdCategory.Moving => "moving",
        AdCategory.ApplianceRepair => "appliance_repair",
        AdCategory.Lending => "lending",
        AdCategory.Cleaning => "cleaning",
        AdCategory.Other => "other",
        _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
    };

    public static AdStatus MapStatus(string value) => value switch
    {
        "active" => AdStatus.Active,
        "archived" => AdStatus.Archived,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    public static string ToDbStatus(AdStatus status) => status switch
    {
        AdStatus.Active => "active",
        AdStatus.Archived => "archived",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
    };
}