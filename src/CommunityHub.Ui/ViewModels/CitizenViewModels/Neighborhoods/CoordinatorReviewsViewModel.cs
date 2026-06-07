using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using CommunityHub.Application.Services.Entities.Neighborhoods.Reviews;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class CoordinatorReviewsViewModel : BaseViewModel
{
    private readonly CoordinatorReviewService _service;
    private readonly long _neighborhoodId;
    private readonly long _currentCitizenId;
    private readonly long _coordinatorId;
    private ObservableCollection<CoordinatorReviewItemViewModel> _reviews = new();
    private string _resultsText = string.Empty;
    private string _coordinatorName = string.Empty;

    public CoordinatorReviewsViewModel(
        CoordinatorReviewService service,
        long neighborhoodId,
        long currentCitizenId,
        long coordinatorId,
        string coordinatorName)
    {
        _service = service;
        _neighborhoodId = neighborhoodId;
        _currentCitizenId = currentCitizenId;
        _coordinatorId = coordinatorId;
        _coordinatorName = coordinatorName;
        LoadReviews();
    }

    public ObservableCollection<CoordinatorReviewItemViewModel> Reviews
    {
        get => _reviews;
        private set => SetProperty(ref _reviews, value);
    }

    public string ResultsText
    {
        get => _resultsText;
        private set => SetProperty(ref _resultsText, value);
    }

    public string CoordinatorName => _coordinatorName;

    public void LoadReviews()
    {
        var items = _service.GetByNeighborhood(_neighborhoodId, _currentCitizenId)
            .Select(dto => new CoordinatorReviewItemViewModel(dto))
            .ToList();
        Reviews = new ObservableCollection<CoordinatorReviewItemViewModel>(items);
        ResultsText = $"Showing {items.Count} reviews";
    }

    public (bool success, string? error) ReportReview(long reviewId)
    {
        var result = _service.ReportReview(reviewId, _currentCitizenId);
        if (result.success) LoadReviews();
        return result;
    }
}
