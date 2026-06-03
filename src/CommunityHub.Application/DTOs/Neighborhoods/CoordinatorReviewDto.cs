using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.DTOs.Neighborhoods;

public class CoordinatorReviewDto
{
    public long Id { get; set; }
    public long CitizenId { get; set; }
    public string CitizenFullName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
    public int ReportCount { get; set; }
    public bool IsRemoved { get; set; }
    public bool CanReport { get; set; }
}
