using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings.CommonRooms;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;

public static class CommonRoomRequestMapper
{
    public static CommonRoomRequest Map(IDataReader reader, User tenant)
    {
        CommonRoom commonRoom = MapCommonRoom(reader);
        CommonRoomRequestStatus status = MapStatus(reader);

        return new CommonRoomRequest(
            id: Convert.ToInt64(reader["id"]),
            commonRoom: commonRoom,
            tenant: tenant,
            dateFrom: DateTime.Parse(reader["date_from"].ToString()!),
            dateTo: DateTime.Parse(reader["date_to"].ToString()!),
            status: status,
            approvedDate: MapNullableDate(reader, "approved_date"),
            proposedDateFrom: MapNullableDate(reader, "proposed_date_from"),
            proposedDateTo: MapNullableDate(reader, "proposed_date_to")
        );
    }

    private static CommonRoom MapCommonRoom(IDataReader reader)
    {
        RentalType rentalType = reader["rental_type"].ToString() == "per_day"
            ? RentalType.PerDay
            : RentalType.MultiDay;

        return new CommonRoom(
            Convert.ToInt64(reader["cr_id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            Convert.ToInt32(reader["floor_number"]),
            rentalType,
            Convert.ToInt64(reader["building_id"])
        );
    }

    private static CommonRoomRequestStatus MapStatus(IDataReader reader)
    {
        return reader["status"].ToString() switch
        {
            "pending" => CommonRoomRequestStatus.Pending,
            "approved" => CommonRoomRequestStatus.Approved,
            "rejected" => CommonRoomRequestStatus.Rejected,
            "pending_date_change" => CommonRoomRequestStatus.PendingDateChange,
            _ => throw new ArgumentException($"Unknown status: {reader["status"]}")
        };
    }

    private static DateTime? MapNullableDate(IDataReader reader, string column)
    {
        return reader[column] == DBNull.Value
            ? null
            : DateTime.Parse(reader[column].ToString()!);
    }
}