using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class NeighborhoodReviewItemViewModel
{
    public int Rating { get; }
    public string StarDisplay { get; }
    public string CommentDisplay { get; }
    public string DateDisplay { get; }
    public string RatingDisplay { get; }

    public NeighborhoodReviewItemViewModel(CoordinatorReviewDto dto)
    {
        Rating = dto.Rating;
        StarDisplay = new string('★', dto.Rating) + new string('☆', 5 - dto.Rating);
        CommentDisplay = string.IsNullOrWhiteSpace(dto.Comment) ? "(No comment)" : dto.Comment;
        DateDisplay = dto.CreatedAt;
        RatingDisplay = $"{dto.Rating}/5";
    }
}

public class NeighborhoodReviewsViewModel : BaseViewModel
{
    private readonly CoordinatorReviewService _reviewService;
    private readonly NeighborhoodService _neighborhoodService;
    private readonly long _coordinatorId;

    private ObservableCollection<NeighborhoodDto> _neighborhoods = new();
    private NeighborhoodDto? _selectedNeighborhood;
    private ObservableCollection<NeighborhoodReviewItemViewModel> _reviews = new();
    private string _averageRating = "—";
    private string _averageStarDisplay = string.Empty;
    private bool _hasReviews;

    public NeighborhoodReviewsViewModel(CoordinatorReviewService reviewService,
        NeighborhoodService neighborhoodService, long coordinatorId)
    {
        _reviewService = reviewService;
        _neighborhoodService = neighborhoodService;
        _coordinatorId = coordinatorId;
        LoadNeighborhoods();
    }

    public ObservableCollection<NeighborhoodDto> Neighborhoods
    {
        get => _neighborhoods;
        private set => SetProperty(ref _neighborhoods, value);
    }

    public NeighborhoodDto? SelectedNeighborhood
    {
        get => _selectedNeighborhood;
        set
        {
            SetProperty(ref _selectedNeighborhood, value);
            if (value != null) LoadReviews(value.Id);
        }
    }

    public ObservableCollection<NeighborhoodReviewItemViewModel> Reviews
    {
        get => _reviews;
        private set => SetProperty(ref _reviews, value);
    }

    public string AverageRating
    {
        get => _averageRating;
        private set => SetProperty(ref _averageRating, value);
    }

    public string AverageStarDisplay
    {
        get => _averageStarDisplay;
        private set => SetProperty(ref _averageStarDisplay, value);
    }

    public bool HasReviews
    {
        get => _hasReviews;
        private set => SetProperty(ref _hasReviews, value);
    }

    private void LoadNeighborhoods()
    {
        var neighborhoods = _neighborhoodService.GetByCoordinator(_coordinatorId);
        Neighborhoods = new ObservableCollection<NeighborhoodDto>(neighborhoods);
        if (Neighborhoods.Count > 0)
            SelectedNeighborhood = Neighborhoods[0];
    }

    private void LoadReviews(long neighborhoodId)
    {
        var reviews = _reviewService.GetByNeighborhood(neighborhoodId, 0);

        Reviews = new ObservableCollection<NeighborhoodReviewItemViewModel>(
            reviews.Select(r => new NeighborhoodReviewItemViewModel(r)));

        HasReviews = reviews.Count > 0;

        var lastYear = reviews
            .Where(r => DateTime.TryParse(r.CreatedAt,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out DateTime dt)
                && dt >= DateTime.Today.AddYears(-1))
            .ToList();

        if (lastYear.Count > 0)
        {
            double avg = lastYear.Average(r => r.Rating);
            AverageRating = $"{avg:F1}/5";
            int rounded = (int)Math.Round(avg);
            AverageStarDisplay = new string('★', rounded) + new string('☆', 5 - rounded);
        }
        else
        {
            AverageRating = "No ratings in the last year";
            AverageStarDisplay = string.Empty;
        }
    }
}