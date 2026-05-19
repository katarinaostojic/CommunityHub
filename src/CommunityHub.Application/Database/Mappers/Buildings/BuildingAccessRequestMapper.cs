using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings;

public static class BuildingAccessRequestMapper
{
    public static BuildingAccessRequest MapWithoutBuilding(IDataReader reader)
    {
        string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
            ? null : reader["rejection_reason"].ToString();

        return new BuildingAccessRequest(
            Convert.ToInt64(reader["id"]),
            UserMapper.Map(reader),
            null!,
            reader["unit_number"].ToString()!,
            DateTime.Parse(reader["created_at"].ToString()!),
            RequestStatusMapper.Parse(reader["status"].ToString()!),
            rejectionReason
        );
    }

    public static BuildingAccessRequest MapWithBuilding(IDataReader reader)
    {
        string? rejectionReason = reader.IsDBNull(reader.GetOrdinal("rejection_reason"))
            ? null : reader["rejection_reason"].ToString();

        return new BuildingAccessRequest(
            Convert.ToInt64(reader["id"]),
            UserMapper.Map(reader),
            BuildingMapper.Map(reader, "building_id"),
            reader["unit_number"].ToString()!,
            DateTime.Parse(reader["created_at"].ToString()!),
            RequestStatusMapper.Parse(reader["status"].ToString()!),
            rejectionReason
        );
    }
}