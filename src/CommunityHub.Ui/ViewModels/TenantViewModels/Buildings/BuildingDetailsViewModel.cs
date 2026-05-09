using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BuildingDetailsViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private int _currentImageIndex = 0;

    private string _imageCounterText = string.Empty;
    private List<string> _imageDotColors = new();
    private int _pendingRequestsCount;

    public BuildingDetailsViewModel(Building building, BuildingAccessRequestService requestService)
    {
        Building = building;
        _requestService = requestService;
        _pendingRequestsCount = _requestService.GetPendingRequestsCount(building.Id);
        UpdateImageState();
    }

    public Building Building { get; }

    public string AddressText => $"{Building.Street} {Building.StreetNumber}";
    public string CityCountryText => $"{Building.City.Name}, {Building.City.Country.Name}";
    public int NumberOfFloors => Building.NumberOfFloors;
    public int TotalUnits => Building.TotalUnits;
    public int VacancyCount => Building.VacancyCount;

    public int PendingRequestsCount
    {
        get => _pendingRequestsCount;
        private set => SetProperty(ref _pendingRequestsCount, value);
    }

    public string ImageCounterText
    {
        get => _imageCounterText;
        private set => SetProperty(ref _imageCounterText, value);
    }

    public List<string> ImageDotColors
    {
        get => _imageDotColors;
        private set => SetProperty(ref _imageDotColors, value);
    }

    public bool HasImages => Building.Images.Count > 0;

    public string? CurrentImagePath => HasImages
        ? Building.Images[_currentImageIndex].Path
        : null;

    public void NextImage()
    {
        if (!HasImages) return;
        _currentImageIndex = (_currentImageIndex + 1) % Building.Images.Count;
        UpdateImageState();
    }

    public void PreviousImage()
    {
        if (!HasImages) return;
        _currentImageIndex = (_currentImageIndex - 1 + Building.Images.Count) % Building.Images.Count;
        UpdateImageState();
    }

    public void RefreshPendingRequestsCount()
    {
        PendingRequestsCount = _requestService.GetPendingRequestsCount(Building.Id);
    }

    private void UpdateImageState()
    {
        if (!HasImages) return;

        ImageCounterText = $"{_currentImageIndex + 1}/{Building.Images.Count}";
        ImageDotColors = Building.Images
            .Select((_, index) => index == _currentImageIndex ? "White" : "#88FFFFFF")
            .ToList();

        OnPropertyChanged(nameof(CurrentImagePath));
    }
}