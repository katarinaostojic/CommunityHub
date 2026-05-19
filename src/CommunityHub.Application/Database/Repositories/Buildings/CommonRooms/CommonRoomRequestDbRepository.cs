using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Mappers.Buildings.CommonRooms;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.Buildings.CommonRooms;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.CommonRooms;

public class CommonRoomRequestDbRepository : BaseDbRepository, ICommonRoomRequestRepository
{
    private static readonly UserColumnAliases TenantAliases = new(
        "user_id",
        "tenant_username",
        "tenant_password",
        "tenant_name",
        "tenant_surname",
        "tenant_birthday",
        "tenant_role");

    public List<CommonRoomRequest> GetAllByCommonRoom(long commonRoomId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT r.id, r.date_from, r.date_to, r.status, 
               r.approved_date, r.proposed_date_from, r.proposed_date_to,
               cr.id AS cr_id, cr.name, cr.description, cr.floor_number, 
               cr.rental_type, cr.building_id,
               u.id AS user_id, u.username AS tenant_username, 
               u.password AS tenant_password,
               u.name AS tenant_name, u.surname AS tenant_surname, 
               u.birthday AS tenant_birthday, u.role AS tenant_role
        FROM common_room_requests r
        JOIN common_rooms cr ON r.common_room_id = cr.id
        JOIN users u ON r.tenant_id = u.id
        WHERE r.common_room_id = @commonRoomId
        ORDER BY r.id DESC";

        AddParameter(command, "@commonRoomId", commonRoomId);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public CommonRoomRequest? GetById(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT r.id, r.date_from, r.date_to, r.status,
               r.approved_date, r.proposed_date_from, r.proposed_date_to,
               cr.id AS cr_id, cr.name, cr.description, cr.floor_number,
               cr.rental_type, cr.building_id,
               u.id AS user_id, u.username AS tenant_username,
               u.password AS tenant_password,
               u.name AS tenant_name, u.surname AS tenant_surname,
               u.birthday AS tenant_birthday, u.role AS tenant_role
        FROM common_room_requests r
        JOIN common_rooms cr ON r.common_room_id = cr.id
        JOIN users u ON r.tenant_id = u.id
        WHERE r.id = @requestId";

        AddParameter(command, "@requestId", requestId);

        using IDataReader reader = command.ExecuteReader();
        return ReadSingleRequest(reader);
    }

    public List<CommonRoomRequest> GetByTenant(long tenantId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT r.id, r.date_from, r.date_to, r.status,
               r.approved_date, r.proposed_date_from, r.proposed_date_to,
               cr.id AS cr_id, cr.name, cr.description, cr.floor_number,
               cr.rental_type, cr.building_id,
               u.id AS user_id, u.username AS tenant_username,
               u.password AS tenant_password,
               u.name AS tenant_name, u.surname AS tenant_surname,
               u.birthday AS tenant_birthday, u.role AS tenant_role
        FROM common_room_requests r
        JOIN common_rooms cr ON r.common_room_id = cr.id
        JOIN users u ON r.tenant_id = u.id
        WHERE r.tenant_id = @tenantId
        ORDER BY r.id DESC";

        AddParameter(command, "@tenantId", tenantId);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public List<CommonRoomRequest> GetByTenantAndBuilding(long tenantId, long buildingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT r.id, r.date_from, r.date_to, r.status,
               r.approved_date, r.proposed_date_from, r.proposed_date_to,
               cr.id AS cr_id, cr.name, cr.description, cr.floor_number,
               cr.rental_type, cr.building_id,
               u.id AS user_id, u.username AS tenant_username,
               u.password AS tenant_password,
               u.name AS tenant_name, u.surname AS tenant_surname,
               u.birthday AS tenant_birthday, u.role AS tenant_role
        FROM common_room_requests r
        JOIN common_rooms cr ON r.common_room_id = cr.id
        JOIN users u ON r.tenant_id = u.id
        WHERE r.tenant_id = @tenantId
          AND cr.building_id = @buildingId
        ORDER BY r.id DESC";

        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@buildingId", buildingId);

        using IDataReader reader = command.ExecuteReader();
        return ReadRequests(reader);
    }

    public long Create(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO common_room_requests
        (common_room_id, tenant_id, date_from, date_to)
        VALUES
        (@commonRoomId, @tenantId, @dateFrom, @dateTo)
        RETURNING id";

        AddParameter(command, "@commonRoomId", commonRoomId);
        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@dateFrom", dateFrom);
        AddParameter(command, "@dateTo", dateTo);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void Update(CommonRoomRequest request)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE common_room_requests
        SET date_from = @dateFrom,
            date_to = @dateTo,
            status = @status::common_room_request_status,
            approved_date = @approvedDate,
            proposed_date_from = @proposedDateFrom,
            proposed_date_to = @proposedDateTo
        WHERE id = @id";

        AddParameter(command, "@id", request.Id);
        AddParameter(command, "@dateFrom", request.DateFrom);
        AddParameter(command, "@dateTo", request.DateTo);
        AddParameter(command, "@status", CommonRoomRequestStatusMapper.ToDatabaseValue(request.Status));
        AddParameter(command, "@approvedDate", request.ApprovedDate);
        AddParameter(command, "@proposedDateFrom", request.ProposedDateFrom);
        AddParameter(command, "@proposedDateTo", request.ProposedDateTo);

        command.ExecuteNonQuery();
    }

    public void Delete(long requestId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = "DELETE FROM common_room_requests WHERE id = @id";

        AddParameter(command, "@id", requestId);
        command.ExecuteNonQuery();
    }

    private static CommonRoomRequest? ReadSingleRequest(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return ReadRequest(reader);
    }

    private static List<CommonRoomRequest> ReadRequests(IDataReader reader)
    {
        List<CommonRoomRequest> requests = new();

        while (reader.Read())
        {
            requests.Add(ReadRequest(reader));
        }

        return requests;
    }

    private static CommonRoomRequest ReadRequest(IDataReader reader)
    {
        User tenant = UserMapper.MapWithAliases(reader, TenantAliases);
        return CommonRoomRequestMapper.Map(reader, tenant);
    }
}