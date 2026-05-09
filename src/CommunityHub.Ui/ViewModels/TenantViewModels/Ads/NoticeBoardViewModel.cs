using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class NoticeBoardViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _currentUserId;
    private readonly long _buildingId;

    private List<Ad> _allActiveAds = new();
    private ObservableCollection<AdViewModel> _filteredAds = new();
    private AdType? _currentTypeFilter = null;
    private AdCategory? _currentCategoryFilter = null;
    private string _resultsCountText = string.Empty;

    public NoticeBoardViewModel(AdService adService, BuildingMembership membership, long currentUserId)
    {
        _adService = adService;
        _currentUserId = currentUserId;
        _buildingId = membership.Building.Id;
        BuildingSubtitle = $"Building: {membership.Building.Street} {membership.Building.StreetNumber}, {membership.Building.Neighborhood}";
        CategoryOptions = BuildCategoryOptions();
        LoadAds();
    }

    public string BuildingSubtitle { get; }
    public List<string> CategoryOptions { get; }

    public ObservableCollection<AdViewModel> FilteredAds
    {
        get => _filteredAds;
        private set => SetProperty(ref _filteredAds, value);
    }

    public string ResultsCountText
    {
        get => _resultsCountText;
        private set => SetProperty(ref _resultsCountText, value);
    }

    public void FilterAll()
    {
        _currentTypeFilter = null;
        ApplyFilters();
    }

    public void FilterOffering()
    {
        _currentTypeFilter = AdType.Offering;
        ApplyFilters();
    }

    public void FilterSeeking()
    {
        _currentTypeFilter = AdType.Seeking;
        ApplyFilters();
    }

    public void FilterByCategory(int selectedIndex)
    {
        _currentCategoryFilter = selectedIndex == 0 ? null : (AdCategory)(selectedIndex - 1);
        ApplyFilters();
    }

    public void ArchiveAd(long adId)
    {
        _adService.Archive(adId);
        LoadAds();
    }

    public void RestoreAd(long adId)
    {
        _adService.Restore(adId);
        LoadAds();
    }

    public Ad? GetAdById(long adId) => _adService.GetById(adId);

    private void LoadAds()
    {
        _allActiveAds = _adService.GetActiveByBuilding(_buildingId);
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        List<AdViewModel> filtered = _allActiveAds
            .Where(ad => _currentTypeFilter == null || ad.Type == _currentTypeFilter)
            .Where(ad => _currentCategoryFilter == null || ad.Category == _currentCategoryFilter)
            .Select(ad => new AdViewModel(ad, _currentUserId))
            .ToList();

        FilteredAds = new ObservableCollection<AdViewModel>(filtered);
        ResultsCountText = $"Showing {filtered.Count} active ad{(filtered.Count != 1 ? "s" : "")}";
    }

    private static List<string> BuildCategoryOptions()
    {
        List<string> options = new() { "All Categories" };
        foreach (AdCategory category in Enum.GetValues<AdCategory>())
            options.Add(category.ToDisplayString());
        return options;
    }
}