using CommunityHub.Application.Database.Mappers.Buildings.ProblemReports;
using CommunityHub.Application.Database.Mappers.Users;
using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Application.Domain.Entities.Shared;
using System.Data;

namespace CommunityHub.Application.Database.Readers.Buildings.ProblemReports;

public static class ProblemReportReader
{
    public static List<ProblemReport> ReadReports(IDataReader reader)
    {
        List<ProblemReport> reports = new();

        while (reader.Read())
        {
            reports.Add(ReadReport(reader));
        }

        return reports;
    }

    public static ProblemReport? ReadSingleReport(IDataReader reader)
    {
        if (!reader.Read())
        {
            return null;
        }

        return ReadReport(reader);
    }

    private static ProblemReport ReadReport(IDataReader reader)
    {
        User tenant = UserMapper.Map(reader);
        return ProblemReportMapper.Map(reader, tenant);
    }
}