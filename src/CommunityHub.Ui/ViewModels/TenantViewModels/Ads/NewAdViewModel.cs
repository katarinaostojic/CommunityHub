using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class NewAdViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _buildingId;
    private readonly long _authorId;

    private AdType _selectedType = AdType.Offering;
    private string _descriptionError = string.Empty;
    private bool _hasDescriptionError;
    private string _dateError = string.Empty;
    private bool _hasDateError;

    public NewAdViewModel(AdService adService, BuildingMembership membership, long authorId)
    {
        _adService = adService;
        _buildingId = membership.Building.Id;
        _authorId = authorId;
        BuildingSubtitle = $"Building: {membership.Building.Street} {membership.Building.StreetNumber}, {membership.Building.Neighborhood}";
        CategoryOptions = Enum.GetValues<AdCategory>()
            .Select(c => c.ToDisplayString())
            .ToList();
    }

    public string BuildingSubtitle { get; }
    public List<string> CategoryOptions { get; }

    public AdType SelectedType
    {
        get => _selectedType;
        private set
        {
            if (SetProperty(ref _selectedType, value))
            {
                OnPropertyChanged(nameof(IsOfferingSelected));
                OnPropertyChanged(nameof(IsSeekingSelected));
            }
        }
    }

    public bool IsOfferingSelected => SelectedType == AdType.Offering;
    public bool IsSeekingSelected => SelectedType == AdType.Seeking;

    public string DescriptionError
    {
        get => _descriptionError;
        private set => SetProperty(ref _descriptionError, value);
    }

    public bool HasDescriptionError
    {
        get => _hasDescriptionError;
        private set => SetProperty(ref _hasDescriptionError, value);
    }

    public string DateError
    {
        get => _dateError;
        private set => SetProperty(ref _dateError, value);
    }

    public bool HasDateError
    {
        get => _hasDateError;
        private set => SetProperty(ref _hasDateError, value);
    }

    public void SelectOffering() => SelectedType = AdType.Offering;
    public void SelectSeeking() => SelectedType = AdType.Seeking;

    public (Ad newAd, List<Ad> matchingAds)? TryCreateAd(
        string description, DateTime? dateFrom, DateTime? dateTo, int categoryIndex)
    {
        if (!Validate(description, dateFrom, dateTo)) return null;

        DateOnly from = DateOnly.FromDateTime(dateFrom!.Value);
        DateOnly to = DateOnly.FromDateTime(dateTo!.Value);
        AdCategory category = (AdCategory)categoryIndex;

        Ad newAd = _adService.Create(_buildingId, _authorId, SelectedType, category, description.Trim(), from, to);
        List<Ad> matchingAds = _adService.FindMatchingAds(newAd);

        return (newAd, matchingAds);
    }

    private bool Validate(string description, DateTime? dateFrom, DateTime? dateTo)
    {
        HasDescriptionError = string.IsNullOrWhiteSpace(description);
        DescriptionError = HasDescriptionError ? "Please enter a description." : string.Empty;

        string? dateError = dateFrom == null || dateTo == null
            ? "Please select a date range."
            : Ad.ValidateDateRange(DateOnly.FromDateTime(dateFrom.Value), DateOnly.FromDateTime(dateTo.Value));

        HasDateError = dateError != null;
        DateError = dateError ?? string.Empty;

        return !HasDescriptionError && !HasDateError;
    }
}