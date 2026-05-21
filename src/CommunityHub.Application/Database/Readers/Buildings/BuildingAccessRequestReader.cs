using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings;

public static class BuildingAccessRequestReader
{
    public static List<BuildingAccessRequest> ReadRequests(IDataReader reader)
    {
        List<BuildingAccessRequest> requests = new();

        while (reader.Read())
        {
            requests.Add(BuildingAccessRequestMapper.MapWithBuilding(reader));
        }

        return requests;
    }

    public static BuildingAccessRequest? ReadSingleRequest(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return BuildingAccessRequestMapper.MapWithBuilding(reader);
    }
}