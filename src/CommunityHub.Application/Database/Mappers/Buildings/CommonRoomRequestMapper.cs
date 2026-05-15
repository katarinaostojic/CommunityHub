using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings;

public static class CommonRoomRequestMapper
{
    public static CommonRoomRequest Map(IDataReader reader, User tenant)
    {
        RentalType rentalType = reader["rental_type"].ToString() == "per_day"
            ? RentalType.PerDay
            : RentalType.MultiDay;

        CommonRoom commonRoom = new CommonRoom(
            Convert.ToInt64(reader["cr_id"]),
            reader["name"].ToString()!,
            reader["description"].ToString()!,
            Convert.ToInt32(reader["floor_number"]),
            rentalType,
            Convert.ToInt64(reader["building_id"])
        );

        CommonRoomRequestStatus status = reader["status"].ToString() switch
        {
            "pending" => CommonRoomRequestStatus.Pending,
            "approved" => CommonRoomRequestStatus.Approved,
            "rejected" => CommonRoomRequestStatus.Rejected,
            "pending_date_change" => CommonRoomRequestStatus.PendingDateChange,
            _ => throw new ArgumentException($"Unknown status: {reader["status"]}")
        };

        DateTime? approvedDate = reader["approved_date"] == DBNull.Value
            ? null : DateTime.Parse(reader["approved_date"].ToString()!);

        DateTime? proposedDateFrom = reader["proposed_date_from"] == DBNull.Value
            ? null : DateTime.Parse(reader["proposed_date_from"].ToString()!);

        DateTime? proposedDateTo = reader["proposed_date_to"] == DBNull.Value
            ? null : DateTime.Parse(reader["proposed_date_to"].ToString()!);

        return new CommonRoomRequest(
            id: Convert.ToInt64(reader["id"]),
            commonRoom: commonRoom,
            tenant: tenant,
            dateFrom: DateTime.Parse(reader["date_from"].ToString()!),
            dateTo: DateTime.Parse(reader["date_to"].ToString()!),
            status: status,
            approvedDate: approvedDate,
            proposedDateFrom: proposedDateFrom,
            proposedDateTo: proposedDateTo
        );
    }
}