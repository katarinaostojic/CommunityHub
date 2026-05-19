using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings;

public static class BuildingAccessRequestReader
{
    public static List<BuildingAccessRequest> ReadAccessRequests(IDataReader reader)
    {
        List<BuildingAccessRequest> requests = new();

        while (reader.Read())
        {
            requests.Add(BuildingAccessRequestMapper.MapWithoutBuilding(reader));
        }

        return requests;
    }
}