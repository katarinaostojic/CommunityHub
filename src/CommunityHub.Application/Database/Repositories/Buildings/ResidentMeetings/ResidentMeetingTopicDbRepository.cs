using CommunityHub.Application.Database.Readers.Buildings.ResidentMeetings;
using CommunityHub.Application.Domain.Entities.Buildings.ResidentMeetings;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.ResidentMeetings;

public partial class ResidentMeetingDbRepository
{
    public void CreateTopicSuggestion(ResidentMeetingTopicSuggestion suggestion)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO resident_meeting_topic_suggestions
            (meeting_id, tenant_id, topic, suggested_at)
        VALUES
            (@meetingId, @tenantId, @topic, @suggestedAt)";

        AddParameter(command, "@meetingId", suggestion.MeetingId);
        AddParameter(command, "@tenantId", suggestion.TenantId);
        AddParameter(command, "@topic", suggestion.Topic);
        AddTimestampParameter(command, "@suggestedAt", suggestion.SuggestedAt);

        command.ExecuteNonQuery();
    }

    public void AddTopic(long meetingId, string topic)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO resident_meeting_topics (meeting_id, topic)
        VALUES (@meetingId, @topic)";

        AddParameter(command, "@meetingId", meetingId);
        AddParameter(command, "@topic", topic);

        command.ExecuteNonQuery();
    }

    public List<ResidentMeetingTopicSuggestion> GetTopicSuggestions(long meetingId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT id, meeting_id, tenant_id, topic, suggested_at
        FROM resident_meeting_topic_suggestions
        WHERE meeting_id = @meetingId
        ORDER BY suggested_at";

        AddParameter(command, "@meetingId", meetingId);

        using IDataReader reader = command.ExecuteReader();
        return ResidentMeetingReader.ReadTopicSuggestions(reader);
    }
}