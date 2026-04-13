using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Data;

namespace CommunityHub.Application.Database.Repositories;

public class BuildingMembershipDbRepository : BaseDbRepository
{
    private readonly ImageDbRepository _imageRepository;

    public BuildingMembershipDbRepository()
    {
        _imageRepository = new ImageDbRepository();
    }

    public List<BuildingMembership> GetByTenant(long tenantId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT bm.id, bm.unit_number, bm.floor_number, bm.approved_at,
                   b.id AS building_id, b.street, b.street_number, b.neighborhood, b.number_of_floors,
                   c.id AS city_id, c.name AS city_name,
                   co.id AS country_id, co.name AS country_name, co.code AS country_code,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_memberships bm
            JOIN buildings b ON bm.building_id = b.id
            JOIN cities c ON b.city_id = c.id
            JOIN countries co ON c.country_id = co.id
            JOIN users u ON bm.user_id = u.id
            WHERE bm.user_id = @userId
            ORDER BY bm.approved_at DESC";

        AddParameter(command, "@userId", tenantId);

        using IDataReader reader = command.ExecuteReader();
        List<BuildingMembership> memberships = new List<BuildingMembership>();
        while (reader.Read())
            memberships.Add(BuildingMembershipMapper.MapWithBuilding(reader));

        Dictionary<long, List<Image>> imageMap =
            _imageRepository.GetByEntities("building", memberships.Select(m => m.Building.Id));
        foreach (BuildingMembership m in memberships)
            foreach (Image image in imageMap[m.Building.Id])
                m.Building.AddImage(image);

        return memberships;
    }

    public List<BuildingMembership> GetByBuilding(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT bm.id, bm.unit_number, bm.floor_number, bm.approved_at,
                   u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
            FROM building_memberships bm
            JOIN users u ON bm.user_id = u.id
            WHERE bm.building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        List<BuildingMembership> memberships = new List<BuildingMembership>();
        while (reader.Read())
            memberships.Add(BuildingMembershipMapper.MapWithoutBuilding(reader));
        return memberships;
    }

    public List<string> GetOccupiedUnits(long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT unit_number FROM building_memberships
            WHERE building_id = @buildingId";

        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        List<string> result = new List<string>();
        while (reader.Read())
            result.Add(reader["unit_number"].ToString()!);
        return result;
    }

    public void Create(BuildingAccessRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand floorCmd = connection.CreateCommand();
        floorCmd.CommandText = @"
            SELECT f.floor_number FROM units u
            JOIN floors f ON u.floor_id = f.id
            WHERE f.building_id = @buildingId AND u.unit_number = @unitNumber
            LIMIT 1";
        AddParameter(floorCmd, "@buildingId", request.Building.Id);
        AddParameter(floorCmd, "@unitNumber", request.UnitNumber);

        object? floorResult = floorCmd.ExecuteScalar();
        int floorNumber = floorResult != null && floorResult != DBNull.Value
            ? Convert.ToInt32(floorResult) : 0;

        IDbCommand cmd = connection.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO building_memberships (building_id, user_id, unit_number, floor_number, approved_at)
            VALUES (@buildingId, @userId, @unitNumber, @floorNumber, @approvedAt)";
        AddParameter(cmd, "@buildingId", request.Building.Id);
        AddParameter(cmd, "@userId", request.User.Id);
        AddParameter(cmd, "@unitNumber", request.UnitNumber);
        AddParameter(cmd, "@floorNumber", floorNumber);
        AddParameter(cmd, "@approvedAt", DateTime.UtcNow);
        cmd.ExecuteNonQuery();
    }
}