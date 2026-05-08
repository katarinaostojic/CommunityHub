using CommunityHub.Application.Domain.Ads;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class MatchingAdViewModel : BaseViewModel
{
    private readonly Ad _ad;
    private readonly Ad _myAd;

    public MatchingAdViewModel(Ad ad, Ad myAd)
    {
        _ad = ad;
        _myAd = myAd;
    }

    public long Id => _ad.Id;
    public string AuthorName => _ad.Author.DisplayName;
    public string Description => _ad.Description;
    public string TypeDisplay => _ad.Type.ToDisplayString();
    public string CategoryDisplay => _ad.Category.ToDisplayString();
    public string DateRangeDisplay => $"📅 {_ad.DateFrom:dd.MM.yyyy} – {_ad.DateTo:dd.MM.yyyy}";

    public string OverlapDisplay
    {
        get
        {
            DateOnly overlapFrom = _ad.DateFrom > _myAd.DateFrom ? _ad.DateFrom : _myAd.DateFrom;
            DateOnly overlapTo = _ad.DateTo < _myAd.DateTo ? _ad.DateTo : _myAd.DateTo;
            return $"Overlap: {overlapFrom:dd.MM.} – {overlapTo:dd.MM.}";
        }
    }
}