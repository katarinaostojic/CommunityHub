using CommunityHub.Application.Database.Readers.Buildings;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings;

public class BuildingMembershipDbRepository : BaseDbRepository, IBuildingMembershipRepository
{
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
        return BuildingMembershipReader.ReadMembershipsWithBuilding(reader);
    }

    public void Create(BuildingAccessRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        int floorNumber = GetFloorNumber(connection, request);
        CreateMembership(connection, request, floorNumber);
    }

    private int GetFloorNumber(IDbConnection connection, BuildingAccessRequest request)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT f.floor_number FROM units u
            JOIN floors f ON u.floor_id = f.id
            WHERE f.building_id = @buildingId AND u.unit_number = @unitNumber
            LIMIT 1";

        AddParameter(command, "@buildingId", request.Building.Id);
        AddParameter(command, "@unitNumber", request.UnitNumber);

        object? result = command.ExecuteScalar();

        if (result == null || result == DBNull.Value)
        {
            return 0;
        }

        return Convert.ToInt32(result);
    }

    private void CreateMembership(
        IDbConnection connection,
        BuildingAccessRequest request,
        int floorNumber)
    {
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO building_memberships (building_id, user_id, unit_number, floor_number, approved_at)
            VALUES (@buildingId, @userId, @unitNumber, @floorNumber, @approvedAt)";

        AddParameter(command, "@buildingId", request.Building.Id);
        AddParameter(command, "@userId", request.Tenant.Id);
        AddParameter(command, "@unitNumber", request.UnitNumber);
        AddParameter(command, "@floorNumber", floorNumber);
        AddParameter(command, "@approvedAt", DateTime.UtcNow);

        command.ExecuteNonQuery();
    }
}