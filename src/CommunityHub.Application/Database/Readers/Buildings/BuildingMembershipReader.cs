using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings;

public static class BuildingMembershipReader
{
    public static List<BuildingMembership> ReadMemberships(IDataReader reader)
    {
        List<BuildingMembership> memberships = new();

        while (reader.Read())
        {
            memberships.Add(BuildingMembershipMapper.MapWithoutBuilding(reader));
        }

        return memberships;
    }

    public static List<BuildingMembership> ReadMembershipsWithBuilding(IDataReader reader)
    {
        List<BuildingMembership> memberships = new();

        while (reader.Read())
        {
            memberships.Add(BuildingMembershipMapper.MapWithBuilding(reader));
        }

        return memberships;
    }
}