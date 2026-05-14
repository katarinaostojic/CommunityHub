using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DTOs.TenantAds;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class NoticeBoardViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _currentUserId;
    private readonly long _buildingId;

    private List<AdDto> _allActiveAds = new();
    private ObservableCollection<AdViewModel> _filteredAds = new();
    private ObservableCollection<AdNotificationViewModel> _notifications = new();

    private AdType? _currentTypeFilter = null;
    private AdCategory? _currentCategoryFilter = null;

    private string _resultsCountText = string.Empty;
    private bool _hasNotifications;

    private string _allFilterText = "All (0)";
    private string _offeringFilterText = "Offering (0)";
    private string _seekingFilterText = "Seeking (0)";

    public NoticeBoardViewModel(AdService adService, BuildingMembershipDto membership, long currentUserId)
    {
        _adService = adService;
        _currentUserId = currentUserId;
        _buildingId = membership.BuildingId;

        BuildingSubtitle = membership.BuildingSubtitle;
        CategoryOptions = BuildCategoryOptions();

        LoadAds();
        LoadNotifications();
    }

    public string BuildingSubtitle { get; }

    public List<string> CategoryOptions { get; }

    public ObservableCollection<AdViewModel> FilteredAds
    {
        get => _filteredAds;
        private set => SetProperty(ref _filteredAds, value);
    }

    public ObservableCollection<AdNotificationViewModel> Notifications
    {
        get => _notifications;
        private set => SetProperty(ref _notifications, value);
    }

    public bool HasNotifications
    {
        get => _hasNotifications;
        private set => SetProperty(ref _hasNotifications, value);
    }

    public string ResultsCountText
    {
        get => _resultsCountText;
        private set => SetProperty(ref _resultsCountText, value);
    }

    public string AllFilterText
    {
        get => _allFilterText;
        private set => SetProperty(ref _allFilterText, value);
    }

    public string OfferingFilterText
    {
        get => _offeringFilterText;
        private set => SetProperty(ref _offeringFilterText, value);
    }

    public string SeekingFilterText
    {
        get => _seekingFilterText;
        private set => SetProperty(ref _seekingFilterText, value);
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
        _currentCategoryFilter = selectedIndex == 0
            ? null
            : (AdCategory)(selectedIndex - 1);

        UpdateFilterCounts();
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

    public AdDto? GetAdById(long adId)
    {
        return _adService.GetById(adId);
    }

    public AdDto? GetMyMatchingAd(AdDto theirAd)
    {
        AdType myType = theirAd.Type == AdType.Offering
            ? AdType.Seeking
            : AdType.Offering;

        return _allActiveAds.FirstOrDefault(ad =>
            ad.AuthorId == _currentUserId
            && ad.Type == myType
            && ad.Category == theirAd.Category
            && ad.OverlapsWith(theirAd.DateFrom, theirAd.DateTo));
    }

    public void DismissNotification(long notificationId)
    {
        AdNotificationViewModel? notification = Notifications
            .FirstOrDefault(n => n.Id == notificationId);

        if (notification == null) return;

        _adService.MarkNotificationAsRead(notificationId);

        Notifications.Remove(notification);
        HasNotifications = Notifications.Count > 0;
    }

    public void MarkAllNotificationsAsRead()
    {
        _adService.MarkAllNotificationsAsRead(_currentUserId);

        Notifications.Clear();
        HasNotifications = false;
    }

    private void LoadAds()
    {
        _allActiveAds = _adService.GetActiveByBuilding(_buildingId);

        UpdateFilterCounts();
        ApplyFilters();
    }

    private void LoadNotifications()
    {
        List<AdNotificationViewModel> notifications = _adService
            .GetUnreadNotifications(_currentUserId)
            .Select(n => new AdNotificationViewModel(n))
            .ToList();

        Notifications = new ObservableCollection<AdNotificationViewModel>(notifications);
        HasNotifications = notifications.Count > 0;
    }

    private void UpdateFilterCounts()
    {
        List<AdDto> adsForSelectedCategory = _allActiveAds
            .Where(ad => _currentCategoryFilter == null || ad.Category == _currentCategoryFilter)
            .ToList();

        int allCount = adsForSelectedCategory.Count;
        int offeringCount = adsForSelectedCategory.Count(ad => ad.Type == AdType.Offering);
        int seekingCount = adsForSelectedCategory.Count(ad => ad.Type == AdType.Seeking);

        AllFilterText = $"All ({allCount})";
        OfferingFilterText = $"Offering ({offeringCount})";
        SeekingFilterText = $"Seeking ({seekingCount})";
    }

    private void ApplyFilters()
    {
        List<AdViewModel> filtered = _allActiveAds
            .Where(ad => _currentTypeFilter == null || ad.Type == _currentTypeFilter)
            .Where(ad => _currentCategoryFilter == null || ad.Category == _currentCategoryFilter)
            .Select(ad => new AdViewModel(ad, _currentUserId, GetMyMatchingAd(ad)?.Id))
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