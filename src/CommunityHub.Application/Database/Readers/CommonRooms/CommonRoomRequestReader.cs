using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings.CommonRooms;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings.CommonRooms;

public static class CommonRoomRequestReader
{
    private static readonly UserColumnAliases TenantAliases = new(
        "user_id",
        "tenant_username",
        "tenant_password",
        "tenant_name",
        "tenant_surname",
        "tenant_birthday",
        "tenant_role");

    public static List<CommonRoomRequest> ReadRequests(IDataReader reader)
    {
        List<CommonRoomRequest> requests = new();

        while (reader.Read())
        {
            requests.Add(ReadRequest(reader));
        }

        return requests;
    }

    public static CommonRoomRequest? ReadSingleRequest(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return ReadRequest(reader);
    }

    private static CommonRoomRequest ReadRequest(IDataReader reader)
    {
        User tenant = UserMapper.MapWithAliases(reader, TenantAliases);
        return CommonRoomRequestMapper.Map(reader, tenant);
    }
}