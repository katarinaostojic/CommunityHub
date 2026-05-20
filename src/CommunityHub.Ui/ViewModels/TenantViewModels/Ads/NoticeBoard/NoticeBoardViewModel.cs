using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Ads;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NoticeBoard;

public class NoticeBoardViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _currentUserId;
    private readonly long _buildingId;

    private ObservableCollection<AdViewModel> _filteredAds = new();
    private string _resultsCountText = string.Empty;

    public NoticeBoardViewModel(
        AdService adService,
        AdNotificationService notificationService,
        BuildingMembershipDto membership,
        long currentUserId)
    {
        _adService = adService;
        _currentUserId = currentUserId;
        _buildingId = membership.BuildingId;

        BuildingSubtitle = membership.BuildingSubtitle;
        Filters = new NoticeBoardFiltersViewModel(adService, _buildingId);
        Notifications = new NoticeBoardNotificationsViewModel(notificationService, currentUserId);

        LoadAds();
    }

    public string BuildingSubtitle { get; }
    public NoticeBoardFiltersViewModel Filters { get; }
    public NoticeBoardNotificationsViewModel Notifications { get; }
    public List<string> CategoryOptions => Filters.CategoryOptions;

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
        Filters.SelectAll();
        LoadAds();
    }

    public void FilterOffering()
    {
        Filters.SelectOffering();
        LoadAds();
    }

    public void FilterSeeking()
    {
        Filters.SelectSeeking();
        LoadAds();
    }

    public void FilterByCategory(int selectedIndex)
    {
        Filters.SelectCategory(selectedIndex);
        LoadAds();
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

    public AdDto? GetAdById(long adId)
    {
        return _adService.GetById(adId);
    }

    public AdDto? GetCurrentUserMatchingAd(AdDto theirAd)
    {
        return _adService.GetCurrentUserMatchingAd(_buildingId, _currentUserId, theirAd);
    }

    private void LoadAds()
    {
        Filters.LoadCounts();
        LoadFilteredAds();
    }

    private void LoadFilteredAds()
    {
        List<AdDto> ads = _adService.GetFilteredActiveByBuilding(
            _buildingId,
            Filters.TypeFilter,
            Filters.CategoryFilter);

        List<AdViewModel> adViewModels = ads
            .Select(CreateAdViewModel)
            .ToList();

        FilteredAds = new ObservableCollection<AdViewModel>(adViewModels);
        UpdateResultsCountText(adViewModels.Count);
    }

    private AdViewModel CreateAdViewModel(AdDto ad)
    {
        long? matchingAdId = GetCurrentUserMatchingAd(ad)?.Id;
        return new AdViewModel(ad, _currentUserId, matchingAdId);
    }

    private void UpdateResultsCountText(int count)
    {
        ResultsCountText = $"Showing {count} active ad{(count != 1 ? "s" : "")}";
    }
}