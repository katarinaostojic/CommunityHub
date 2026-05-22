using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BuildingDetailsViewModel : BaseViewModel
{
    private readonly BuildingAccessRequestService _requestService;
    private int _currentImageIndex = 0;

    private string _imageCounterText = string.Empty;
    private List<string> _imageDotColors = new();
    private int _pendingRequestsCount;

    public BuildingDetailsViewModel(BuildingDto buildingDto, BuildingAccessRequestService requestService)
    {
        BuildingDto = buildingDto;
        _requestService = requestService;
        _pendingRequestsCount = _requestService.GetPendingRequestsCount(BuildingDto.Id);
        UpdateImageState();
    }

    public BuildingDto BuildingDto { get; }

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

    public bool HasImages => BuildingDto.ImagePaths.Count > 0;

    public string? CurrentImagePath => HasImages
        ? BuildingDto.ImagePaths[_currentImageIndex]
        : null;

    public void NextImage()
    {
        if (!HasImages) return;
        _currentImageIndex = (_currentImageIndex + 1) % BuildingDto.ImagePaths.Count;
        UpdateImageState();
    }

    public void PreviousImage()
    {
        if (!HasImages) return;
        _currentImageIndex = (_currentImageIndex - 1 + BuildingDto.ImagePaths.Count) % BuildingDto.ImagePaths.Count;
        UpdateImageState();
    }

    public void RefreshPendingRequestsCount()
    {
        PendingRequestsCount = _requestService.GetPendingRequestsCount(BuildingDto.Id);
    }

    private void UpdateImageState()
    {
        if (!HasImages) return;

        ImageCounterText = $"{_currentImageIndex + 1}/{BuildingDto.ImagePaths.Count}";
        ImageDotColors = BuildingDto.ImagePaths
            .Select((_, index) => index == _currentImageIndex ? "White" : "#88FFFFFF")
            .ToList();

        OnPropertyChanged(nameof(CurrentImagePath));
    }
}