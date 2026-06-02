using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Mappers.Buildings.ProblemReports;

public static class ProblemReportMapper
{
    public static ProblemReport Map(IDataReader reader, User tenant)
    {
        return new ProblemReport(
            id: Convert.ToInt64(reader["id"]),
            buildingId: Convert.ToInt64(reader["building_id"]),
            tenant: tenant,
            description: reader["description"].ToString()!,
            priority: ProblemPriorityMapper.FromDatabaseValue(reader["priority"].ToString()!),
            reportedAt: DateTime.Parse(reader["reported_at"].ToString()!),
            status: ProblemReportStatusMapper.FromDatabaseValue(reader["status"].ToString()!));
    }
}