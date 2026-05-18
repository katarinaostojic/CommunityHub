using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.TenantAds;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Ads;

public class ManagerNoticeBoardDetailsViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _buildingId;
    private List<AdDto> _allAds = new();

    public string BuildingTitle { get; }

    private ObservableCollection<ManagerAdRowViewModel> _ads = new();
    public ObservableCollection<ManagerAdRowViewModel> Ads
    {
        get => _ads;
        private set => SetProperty(ref _ads, value);
    }

    private string _offeringText = string.Empty;
    public string OfferingText
    {
        get => _offeringText;
        private set => SetProperty(ref _offeringText, value);
    }

    private string _seekingText = string.Empty;
    public string SeekingText
    {
        get => _seekingText;
        private set => SetProperty(ref _seekingText, value);
    }

    private string _activeText = string.Empty;
    public string ActiveText
    {
        get => _activeText;
        private set => SetProperty(ref _activeText, value);
    }

    private string _archivedText = string.Empty;
    public string ArchivedText
    {
        get => _archivedText;
        private set => SetProperty(ref _archivedText, value);
    }

    private string _topHelperText = string.Empty;
    public string TopHelperText
    {
        get => _topHelperText;
        private set => SetProperty(ref _topHelperText, value);
    }

    private ObservableCollection<string> _activeByCategoryItems = new();
    public ObservableCollection<string> ActiveByCategoryItems
    {
        get => _activeByCategoryItems;
        private set => SetProperty(ref _activeByCategoryItems, value);
    }

    private ObservableCollection<CategoryStatItem> _categoryStats = new();
    public ObservableCollection<CategoryStatItem> CategoryStats
    {
        get => _categoryStats;
        private set => SetProperty(ref _categoryStats, value);
    }

    public ManagerNoticeBoardDetailsViewModel(long buildingId, string buildingTitle)
    {
        _buildingId = buildingId;
        BuildingTitle = buildingTitle;
        _adService = Injector.CreateInstance<AdService>();
        LoadAds();
    }

    public void LoadAds()
    {
        _allAds = _adService.GetAllByBuilding(_buildingId);
        Ads = new ObservableCollection<ManagerAdRowViewModel>(
            _allAds.Select(a => new ManagerAdRowViewModel(a)).ToList()
        );
    }

    public void LoadStatistics(int? year, int? month)
    {
        List<AdDto> filteredAds = FilterAds(year, month);

        OfferingText = $"Offering help: {_adService.CountByType(filteredAds, AdType.Offering)}";
        SeekingText = $"Seeking help: {_adService.CountByType(filteredAds, AdType.Seeking)}";

        var categoryStats = _adService.GetStatsByCategory(filteredAds);
        CategoryStats = new ObservableCollection<CategoryStatItem>(
            categoryStats.Select(kvp => new CategoryStatItem(
                kvp.Key.ToString(),
                kvp.Value.offering,
                kvp.Value.seeking
            )).ToList()
        );

        var (active, archived) = _adService.GetCurrentState(_allAds);
        ActiveText = $"Active ads: {active}";
        ArchivedText = $"Archived ads: {archived}";

        var activeByCategory = _adService.GetActiveCountByCategory(_allAds);
        ActiveByCategoryItems = new ObservableCollection<string>(
            activeByCategory.Select(kvp => $"{kvp.Key}: {kvp.Value}").ToList()
        );

        User? topHelper = _adService.GetTopHelper(_buildingId);
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
    public string Category { get; }
    public int Offering { get; }
    public int Seeking { get; }

    public CategoryStatItem(string category, int offering, int seeking)
    {
        Category = category;
        Offering = offering;
        Seeking = seeking;
    }
}