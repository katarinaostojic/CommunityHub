using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Mappers;

public static class BuildingMembershipMapper
{
    public static BuildingMembership MapWithBuilding(IDataReader reader)
    {
        return new BuildingMembership(
            Convert.ToInt64(reader["id"]),
            BuildingMapper.MapFromJoin(reader),
            UserMapper.Map(reader),
            reader["unit_number"].ToString()!,
            Convert.ToInt32(reader["floor_number"]),
            DateTime.Parse(reader["approved_at"].ToString()!)
        );
    }

    public static BuildingMembership MapWithoutBuilding(IDataReader reader)
    {
        return new BuildingMembership(
            Convert.ToInt64(reader["id"]),
            null!,
            UserMapper.Map(reader),
            reader["unit_number"].ToString()!,
            Convert.ToInt32(reader["floor_number"]),
            DateTime.Parse(reader["approved_at"].ToString()!)
        );
    }
}
