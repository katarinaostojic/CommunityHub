using CommunityHub.Application.Database.Mappers;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Neighborhoods;

public class EventDbRepository : BaseDbRepository, IEventRepository
{
    public long Create(long organizerId, long neighborhoodId, string name, string description,
        DateOnly eventDate, TimeOnly startTime, int durationMinutes, int minVolunteers)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO events (neighborhood_id, organizer_id, name, description, 
                               event_date, start_time, duration_minutes, min_volunteers, status)
            VALUES (@neighborhoodId, @organizerId, @name, @description,
                    @eventDate, @startTime, @durationMinutes, @minVolunteers, 'preparation')
            RETURNING id";

        AddParameter(command, "@neighborhoodId", neighborhoodId);
        AddParameter(command, "@organizerId", organizerId);
        AddParameter(command, "@name", name);
        AddParameter(command, "@description", description);
        AddParameter(command, "@eventDate", eventDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
        AddParameter(command, "@startTime", startTime.ToTimeSpan());
        AddParameter(command, "@durationMinutes", durationMinutes);
        AddParameter(command, "@minVolunteers", minVolunteers);

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void CreateItem(long eventId, string itemName)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO event_items (event_id, name, is_taken)
            VALUES (@eventId, @name, false)";

        AddParameter(command, "@eventId", eventId);
        AddParameter(command, "@name", itemName);
        command.ExecuteNonQuery();
    }

    public List<Event> GetByNeighborhood(long neighborhoodId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT e.id, e.neighborhood_id, e.name, e.description,
                   e.event_date, e.start_time, e.duration_minutes, e.min_volunteers, e.status,
                   u.id AS organizer_id, u.username, u.password, u.name AS organizer_name,
                   u.surname AS organizer_surname, u.birthday, u.role, u.address
            FROM events e
            JOIN users u ON e.organizer_id = u.id
            WHERE e.neighborhood_id = @neighborhoodId
            ORDER BY e.event_date DESC";

        AddParameter(command, "@neighborhoodId", neighborhoodId);

        using IDataReader reader = command.ExecuteReader();
        return ReadAndAttach(reader);
    }

    public Event? GetById(long eventId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT e.id, e.neighborhood_id, e.name, e.description,
                   e.event_date, e.start_time, e.duration_minutes, e.min_volunteers, e.status,
                   u.id AS organizer_id, u.username, u.password, u.name AS organizer_name,
                   u.surname AS organizer_surname, u.birthday, u.role, u.address
            FROM events e
            JOIN users u ON e.organizer_id = u.id
            WHERE e.id = @eventId";

        AddParameter(command, "@eventId", eventId);

        using IDataReader reader = command.ExecuteReader();
        var ev = ReadAndAttach(reader).FirstOrDefault();
        return ev;
    }

    public void Update(Event ev)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE events SET status = @status WHERE id = @id";

        AddParameter(command, "@id", ev.Id);
        AddParameter(command, "@status", ev.Status.ToString().ToLower());
        command.ExecuteNonQuery();
    }

    public void AddRegistration(long eventId, long citizenId, List<long> itemIds)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand regCmd = connection.CreateCommand();
        regCmd.CommandText = @"
            INSERT INTO event_registrations (event_id, citizen_id, registered_at)
            VALUES (@eventId, @citizenId, NOW())
            RETURNING id";

        AddParameter(regCmd, "@eventId", eventId);
        AddParameter(regCmd, "@citizenId", citizenId);
        long registrationId = Convert.ToInt64(regCmd.ExecuteScalar());

        foreach (long itemId in itemIds)
            RegisterItem(connection, registrationId, itemId);
    }

    public void MarkAttendance(long registrationId, bool attended)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "UPDATE event_registrations SET attended = @attended WHERE id = @id";

        AddParameter(command, "@attended", attended);
        AddParameter(command, "@id", registrationId);
        command.ExecuteNonQuery();
    }

    public List<Event> GetAllForStatusCheck()
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT e.id, e.neighborhood_id, e.name, e.description,
                   e.event_date, e.start_time, e.duration_minutes, e.min_volunteers, e.status,
                   u.id AS organizer_id, u.username, u.password, u.name AS organizer_name,
                   u.surname AS organizer_surname, u.birthday, u.role, u.address
            FROM events e
            JOIN users u ON e.organizer_id = u.id
            WHERE e.status IN ('preparation', 'scheduled')";

        using IDataReader reader = command.ExecuteReader();
        return ReadAndAttach(reader);
    }

    private List<Event> ReadAndAttach(IDataReader reader)
    {
        var events = new List<Event>();
        while (reader.Read())
            events.Add(EventMapper.MapEvent(reader));

        foreach (var ev in events)
        {
            AttachItems(ev);
            AttachRegistrations(ev);
        }

        return events;
    }

    private void RegisterItem(IDbConnection connection, long registrationId, long itemId)
    {
        IDbCommand itemCmd = connection.CreateCommand();
        itemCmd.CommandText = @"
            INSERT INTO event_registration_items (registration_id, item_id)
            VALUES (@registrationId, @itemId)";
        AddParameter(itemCmd, "@registrationId", registrationId);
        AddParameter(itemCmd, "@itemId", itemId);
        itemCmd.ExecuteNonQuery();

        IDbCommand updateItemCmd = connection.CreateCommand();
        updateItemCmd.CommandText = "UPDATE event_items SET is_taken = true WHERE id = @itemId";
        AddParameter(updateItemCmd, "@itemId", itemId);
        updateItemCmd.ExecuteNonQuery();
    }

    private void AttachItems(Event ev)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT id, event_id, name, is_taken FROM event_items WHERE event_id = @eventId";

        AddParameter(command, "@eventId", ev.Id);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            ev.AddItem(new EventItem(
                Convert.ToInt64(reader["id"]),
                Convert.ToInt64(reader["event_id"]),
                reader["name"].ToString()!,
                Convert.ToBoolean(reader["is_taken"])
            ));
        }
    }

    private void AttachRegistrations(Event ev)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();
        command.CommandText = @"
            SELECT er.id, er.event_id, er.citizen_id, er.registered_at, er.attended,
                   u.username, u.password, u.name AS citizen_name,
                   u.surname AS citizen_surname, u.birthday, u.role, u.address
            FROM event_registrations er
            JOIN users u ON er.citizen_id = u.id
            WHERE er.event_id = @eventId";

        AddParameter(command, "@eventId", ev.Id);

        using IDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            ev.AddRegistration(new EventRegistration(
                Convert.ToInt64(reader["id"]),
                Convert.ToInt64(reader["event_id"]),
                EventMapper.MapCitizen(reader),
                Convert.ToDateTime(reader["registered_at"]),
                reader.IsDBNull(reader.GetOrdinal("attended")) ? null : Convert.ToBoolean(reader["attended"])
            ));
        }
    }
}