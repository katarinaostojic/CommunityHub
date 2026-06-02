using CommunityHub.Application.Database.Mappers.Buildings.ProblemReports;
using CommunityHub.Application.Database.Readers.Buildings.ProblemReports;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.ProblemReports;
using System.Data;

namespace CommunityHub.Application.Database.Repositories.Buildings.ProblemReports;

public class ProblemReportDbRepository : BaseDbRepository, IProblemReportRepository
{
    public List<ProblemReport> GetByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ProblemReportStatus? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT pr.id, pr.building_id, pr.description, pr.priority, pr.reported_at, pr.status,
               u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
        FROM problem_reports pr
        JOIN users u ON pr.tenant_id = u.id
        WHERE pr.tenant_id = @tenantId
          AND pr.building_id = @buildingId
          AND (@status IS NULL OR pr.status = @status::problem_report_status)
        ORDER BY pr.reported_at DESC, pr.id DESC";

        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@status", GetStatusParameterValue(status));

        using IDataReader reader = command.ExecuteReader();
        return ProblemReportReader.ReadReports(reader);
    }

    public int CountByTenantAndBuilding(
        long tenantId,
        long buildingId,
        ProblemReportStatus? status)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT COUNT(*)
        FROM problem_reports
        WHERE tenant_id = @tenantId
          AND building_id = @buildingId
          AND (@status IS NULL OR status = @status::problem_report_status)";

        AddParameter(command, "@tenantId", tenantId);
        AddParameter(command, "@buildingId", buildingId);
        AddParameter(command, "@status", GetStatusParameterValue(status));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public ProblemReport? GetById(long reportId)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        SELECT pr.id, pr.building_id, pr.description, pr.priority, pr.reported_at, pr.status,
               u.id AS user_id, u.username, u.password, u.name, u.surname, u.birthday, u.role
        FROM problem_reports pr
        JOIN users u ON pr.tenant_id = u.id
        WHERE pr.id = @reportId";

        AddParameter(command, "@reportId", reportId);

        using IDataReader reader = command.ExecuteReader();
        return ProblemReportReader.ReadSingleReport(reader);
    }

    public long Create(ProblemReport report)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        INSERT INTO problem_reports
            (building_id, tenant_id, description, priority, reported_at, status)
        VALUES
            (@buildingId, @tenantId, @description, @priority::problem_report_priority,
             @reportedAt, @status::problem_report_status)
        RETURNING id";

        AddParameter(command, "@buildingId", report.BuildingId);
        AddParameter(command, "@tenantId", report.Tenant.Id);
        AddParameter(command, "@description", report.Description);
        AddParameter(command, "@priority", ProblemPriorityMapper.ToDatabaseValue(report.Priority));
        AddParameter(command, "@reportedAt", report.ReportedAt);
        AddParameter(command, "@status", ProblemReportStatusMapper.ToDatabaseValue(report.Status));

        return Convert.ToInt64(command.ExecuteScalar());
    }

    public void Update(ProblemReport report)
    {
        using IDbConnection connection = PostgresConnection.CreateConnection();
        IDbCommand command = connection.CreateCommand();

        command.CommandText = @"
        UPDATE problem_reports
        SET status = @status::problem_report_status
        WHERE id = @id";

        AddParameter(command, "@id", report.Id);
        AddParameter(command, "@status", ProblemReportStatusMapper.ToDatabaseValue(report.Status));

        command.ExecuteNonQuery();
    }

    private static string? GetStatusParameterValue(ProblemReportStatus? status)
    {
        return status.HasValue
            ? ProblemReportStatusMapper.ToDatabaseValue(status.Value)
            : null;
    }
}