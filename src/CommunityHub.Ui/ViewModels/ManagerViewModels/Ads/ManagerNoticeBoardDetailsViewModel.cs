using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.Services.Ads;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;

public class ManagerNoticeBoardDetailsViewModel : BaseViewModel
{
    private readonly AdStatisticsService _statisticsService;
    private readonly long _buildingId;
    private List<AdDto> _allAds = new();

    private ObservableCollection<ManagerAdRowViewModel> _ads = new();
    private string _offeringText = string.Empty;
    private string _seekingText = string.Empty;
    private string _activeText = string.Empty;
    private string _archivedText = string.Empty;
    private string _topHelperText = string.Empty;
    private ObservableCollection<string> _activeByCategoryItems = new();
    private ObservableCollection<CategoryStatItem> _categoryStats = new();

    public ManagerNoticeBoardDetailsViewModel(long buildingId, string buildingTitle)
    {
        _buildingId = buildingId;
        BuildingTitle = buildingTitle;
        _statisticsService = Injector.CreateInstance<AdStatisticsService>();
        LoadAds();
    }

    public string BuildingTitle { get; }

    public ObservableCollection<ManagerAdRowViewModel> Ads
    {
        get => _ads;
        private set => SetProperty(ref _ads, value);
    }

    public string OfferingText
    {
        get => _offeringText;
        private set => SetProperty(ref _offeringText, value);
    }

    public string SeekingText
    {
        get => _seekingText;
        private set => SetProperty(ref _seekingText, value);
    }

    public string ActiveText
    {
        get => _activeText;
        private set => SetProperty(ref _activeText, value);
    }

    public string ArchivedText
    {
        get => _archivedText;
        private set => SetProperty(ref _archivedText, value);
    }

    public string TopHelperText
    {
        get => _topHelperText;
        private set => SetProperty(ref _topHelperText, value);
    }

    public ObservableCollection<string> ActiveByCategoryItems
    {
        get => _activeByCategoryItems;
        private set => SetProperty(ref _activeByCategoryItems, value);
    }

    public ObservableCollection<CategoryStatItem> CategoryStats
    {
        get => _categoryStats;
        private set => SetProperty(ref _categoryStats, value);
    }

    public void LoadAds()
    {
        _allAds = _statisticsService.GetAllByBuilding(_buildingId);

        Ads = new ObservableCollection<ManagerAdRowViewModel>(
            _allAds.Select(a => new ManagerAdRowViewModel(a)).ToList());
    }

    public void LoadStatistics(int? year, int? month)
    {
        List<AdDto> filteredAds = FilterAds(year, month);

        UpdateTypeStatistics(filteredAds);
        UpdateCategoryStatistics(filteredAds);
        UpdateCurrentStateStatistics();
        UpdateTopHelperText();
    }

    private void UpdateTypeStatistics(List<AdDto> ads)
    {
        OfferingText = $"Offering help: {_statisticsService.CountByType(ads, AdType.Offering)}";
        SeekingText = $"Seeking help: {_statisticsService.CountByType(ads, AdType.Seeking)}";
    }

    private void UpdateCategoryStatistics(List<AdDto> ads)
    {
        Dictionary<AdCategory, (int offering, int seeking)> stats =
            _statisticsService.GetStatsByCategory(ads);

        CategoryStats = new ObservableCollection<CategoryStatItem>(
            stats.Select(kvp => new CategoryStatItem(
                kvp.Key.ToString(),
                kvp.Value.offering,
                kvp.Value.seeking)).ToList());
    }

    private void UpdateCurrentStateStatistics()
    {
        var (active, archived) = _statisticsService.GetCurrentState(_allAds);

        ActiveText = $"Active ads: {active}";
        ArchivedText = $"Archived ads: {archived}";

        Dictionary<AdCategory, int> activeByCategory =
            _statisticsService.GetActiveCountByCategory(_allAds);

        ActiveByCategoryItems = new ObservableCollection<string>(
            activeByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToList());
    }

    private void UpdateTopHelperText()
    {
        User? topHelper = _statisticsService.GetTopHelper(_buildingId);

        TopHelperText = topHelper != null
            ? $"{topHelper.Name} {topHelper.Surname}"
            : "No data yet";
    }

    private List<AdDto> FilterAds(int? year, int? month)
    {
        if (!year.HasValue) return _allAds;
        if (!month.HasValue) return FilterByYear(year.Value);
        return FilterByYearAndMonth(year.Value, month.Value);
    }

    private List<AdDto> FilterByYear(int year)
    {
        return _allAds
            .Where(a => a.DateFrom.Year == year || a.DateTo.Year == year)
            .ToList();
    }

    private List<AdDto> FilterByYearAndMonth(int year, int month)
    {
        return _allAds
            .Where(a => MatchesYearAndMonth(a, year, month))
            .ToList();
    }

    private static bool MatchesYearAndMonth(AdDto ad, int year, int month)
    {
        bool fromMatches = ad.DateFrom.Year == year && ad.DateFrom.Month == month;
        bool toMatches = ad.DateTo.Year == year && ad.DateTo.Month == month;
        return fromMatches || toMatches;
    }
}

public class CategoryStatItem
{
    public CategoryStatItem(string category, int offering, int seeking)
    {
        Category = category;
        Offering = offering;
        Seeking = seeking;
    }

    public string Category { get; }
    public int Offering { get; }
    public int Seeking { get; }
}