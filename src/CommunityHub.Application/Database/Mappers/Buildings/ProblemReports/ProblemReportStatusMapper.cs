using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;

namespace CommunityHub.Application.Database.Mappers.Buildings.ProblemReports;

public static class ProblemReportStatusMapper
{
    public static ProblemReportStatus FromDatabaseValue(string value) => value switch
    {
        "unresolved" => ProblemReportStatus.Unresolved,
        "potentially solved" => ProblemReportStatus.PotentiallySolved,
        "solved" => ProblemReportStatus.Solved,
        _ => throw new ArgumentException($"Unknown problem report status: '{value}'")
    };

    public static string ToDatabaseValue(ProblemReportStatus status) => status switch
    {
        ProblemReportStatus.Unresolved => "unresolved",
        ProblemReportStatus.PotentiallySolved => "potentially solved",
        ProblemReportStatus.Solved => "solved",
        _ => throw new ArgumentException($"Unknown problem report status: '{status}'")
    };
}