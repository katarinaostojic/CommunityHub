using CommunityHub.Application.DTOs.Buildings.Ads;
using CommunityHub.Ui.Extensions;
using CommunityHub.Ui.Extensions.Buildings.Ads;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NewAd;

public class MatchingAdViewModel : BaseViewModel
{
    private readonly AdDto _ad;
    private readonly AdDto _myAd;

    public MatchingAdViewModel(AdDto ad, AdDto myAd)
    {
        _ad = ad;
        _myAd = myAd;
    }

    public long Id => _ad.Id;
    public string AuthorName => _ad.AuthorName;
    public string Description => _ad.Description;
    public string TypeDisplay => _ad.Type.ToDisplayString();
    public string CategoryDisplay => _ad.Category.ToDisplayString();
    public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

    public string OverlapDisplay
    {
        get
        {
            (DateOnly overlapFrom, DateOnly overlapTo) = _ad.GetOverlapWith(_myAd);
            return $"Overlap: {overlapFrom:dd.MM.} – {overlapTo:dd.MM.}";
        }
    }
}