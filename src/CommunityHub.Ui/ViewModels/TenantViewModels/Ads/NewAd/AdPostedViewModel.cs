using CommunityHub.Application.Domain.Entities.Ads;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Ads;
using CommunityHub.Ui.Extensions;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NewAd;

public class AdPostedViewModel : BaseViewModel
{
    private readonly AdService _adService;

    public AdPostedViewModel(AdDto postedAd, List<AdDto> matchingAds, BuildingMembershipDto membership, AdService adService)
    {
        _adService = adService;
        PostedAdId = postedAd.Id;
        TypeDisplay = postedAd.Type.ToDisplayString();
        CategoryDisplay = postedAd.Category.ToDisplayString();
        DateRangeDisplay = $"{postedAd.DateFrom:dd.MM.yyyy} – {postedAd.DateTo:dd.MM.yyyy}";
        BuildingDisplay = membership.BuildingFullAddress;
        MatchingAdsTitle = BuildMatchingAdsTitle(postedAd, matchingAds);
        MatchingAds = new ObservableCollection<MatchingAdViewModel>(
            matchingAds.Select(ad => new MatchingAdViewModel(ad, postedAd)));
    }

    public long PostedAdId { get; }
    public string TypeDisplay { get; }
    public string CategoryDisplay { get; }
    public string DateRangeDisplay { get; }
    public string BuildingDisplay { get; }
    public string MatchingAdsTitle { get; }
    public ObservableCollection<MatchingAdViewModel> MatchingAds { get; }

    public (AdDto? theirAd, AdDto? myAd) GetAdsForBooking(long theirAdId)
    {
        AdDto? theirAd = _adService.GetById(theirAdId);
        AdDto? myAd = _adService.GetById(PostedAdId);
        return (theirAd, myAd);
    }

    private static string BuildMatchingAdsTitle(AdDto postedAd, List<AdDto> matchingAds)
    {
        if (matchingAds.Count == 0)
            return "No matching ads found at the moment. You will be notified when one appears.";

        string oppositeLabel = postedAd.OppositeType == AdType.Seeking ? "seeking" : "offering";
        string tenants = matchingAds.Count == 1 ? "1 tenant" : $"{matchingAds.Count} tenants";
        return $"{tenants} {oppositeLabel} help in {postedAd.Category.ToDisplayString()} · overlapping dates";
    }
}