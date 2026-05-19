using CommunityHub.Application.Database.Readers.Buildings;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Shared;
using CommunityHub.Application.Domain.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings;

public class BuildingDetailsDbRepository : BaseDbRepository
{
    private readonly IImageRepository _imageRepository;

    public BuildingDetailsDbRepository(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }

    public void AttachImages(List<Building> buildings)
    {
        if (buildings.Count == 0)
        {
            return;
        }

        Dictionary<long, List<Image>> imageMap =
            _imageRepository.GetByEntities("building", buildings.Select(b => b.Id));

        foreach (Building building in buildings)
        {
            AddImagesFromMap(building, imageMap);
        }
    }

    public void PopulateBuildingDetails(Building building, IDbConnection connection, long buildingId)
    {
        AddImages(building, buildingId);
        AddMemberships(building, connection, buildingId);
        AddAccessRequests(building, connection, buildingId);
    }

    private void AddImages(Building building, long buildingId)
    {
        foreach (Image image in _imageRepository.GetByEntity("building", buildingId))
        {
            building.AddImage(image);
        }
    }

    private void AddMemberships(Building building, IDbConnection connection, long buildingId)
    {
        foreach (BuildingMembership membership in GetMembershipsByBuilding(connection, buildingId))
        {
            building.AddMembership(membership);
        }
    }

    private void AddAccessRequests(Building building, IDbConnection connection, long buildingId)
    {
        foreach (BuildingAccessRequest request in GetAccessRequestsByBuilding(connection, buildingId))
        {
            building.AddAccessRequest(request);
        }
    }

    private List<BuildingMembership> GetMembershipsByBuilding(IDbConnection connection, long buildingId)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT bm.id, bm.unit_number, bm.floor_number, bm.approved_at,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_memberships bm
            JOIN users u ON bm.user_id = u.id
            WHERE bm.building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return BuildingMembershipReader.ReadMemberships(reader);
    }

    private List<BuildingAccessRequest> GetAccessRequestsByBuilding(IDbConnection connection, long buildingId)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT r.id, r.unit_number, r.created_at, r.status, r.rejection_reason,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_access_requests r
            JOIN users u ON r.user_id = u.id
            WHERE r.building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return BuildingAccessRequestWithoutBuildingReader.ReadAccessRequests(reader);
    }

    private static void AddImagesFromMap(
        Building building,
        Dictionary<long, List<Image>> imageMap)
    {
        if (!imageMap.TryGetValue(building.Id, out List<Image>? images))
        {
            return;
        }

        foreach (Image image in images)
        {
            building.AddImage(image);
        }
    }
}