using CommunityHub.Application.Database.Mappers.Buildings;
using CommunityHub.Application.Domain.Entities.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings;

public static class BuildingAccessRequestWithoutBuildingReader
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