using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class CoordinatorReviewItemViewModel : BaseViewModel
{
    private readonly CoordinatorReviewDto _dto;

    public CoordinatorReviewItemViewModel(CoordinatorReviewDto dto)
    {
        _dto = dto;
    }

    public long Id => _dto.Id;
    public long CitizenId => _dto.CitizenId;
    public string CitizenFullName => _dto.CitizenFullName;
    public int Rating => _dto.Rating;
    public string RatingStars => new string('★', _dto.Rating) + new string('☆', 5 - _dto.Rating);
    public string? Comment => _dto.Comment;
    public string CreatedAt => _dto.CreatedAt;
    public int ReportCount => _dto.ReportCount;
    public bool CanReport => _dto.CanReport;
    public string ReportVisibility => _dto.CanReport ? "Visible" : "Collapsed";
    public bool HasReported => _dto.HasReported;
    public string ReportedVisibility => _dto.HasReported ? "Visible" : "Collapsed";
}
