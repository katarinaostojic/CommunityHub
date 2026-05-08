using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

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

        Ad newAd = _adService.CreateAd(_buildingId, _authorId, SelectedType, category, description.Trim(), from, to);
        List<Ad> activeAds = _adService.GetActiveByBuilding(_buildingId);
        List<Ad> matchingAds = _adService.FindMatchingAds(newAd, activeAds);

        return (newAd, matchingAds);
    }

    private bool Validate(string description, DateTime? dateFrom, DateTime? dateTo)
    {
        bool valid = true;

        if (string.IsNullOrWhiteSpace(description))
        {
            DescriptionError = "Please enter a description.";
            HasDescriptionError = true;
            valid = false;
        }
        else
        {
            DescriptionError = string.Empty;
            HasDescriptionError = false;
        }

        if (dateFrom == null || dateTo == null)
        {
            DateError = "Please select a date range.";
            HasDateError = true;
            valid = false;
        }
        else if (dateFrom.Value.Date < DateTime.Today)
        {
            DateError = "Dates cannot be in the past.";
            HasDateError = true;
            valid = false;
        }
        else if (dateFrom.Value > dateTo.Value)
        {
            DateError = "Start date must be before end date.";
            HasDateError = true;
            valid = false;
        }
        else
        {
            DateError = string.Empty;
            HasDateError = false;
        }

        return valid;
    }
}