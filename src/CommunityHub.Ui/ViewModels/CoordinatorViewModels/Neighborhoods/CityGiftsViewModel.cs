using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CityGiftItemViewModel : BaseViewModel
{
    public long Id { get; }
    public string GiftTitle { get; }
    public string AmountDisplay { get; }
    public string Deadline { get; }
    public string StatusDisplay { get; }
    public bool CanApply { get; }
    public bool HasApplied { get; }
    public bool IsAwarded { get; }

    public CityGiftItemViewModel(CityGiftDto dto, int index)
    {
        Id = dto.Id;
        GiftTitle = $"City gift #{index}";
        AmountDisplay = $"Amount: {dto.AmountDisplay}";
        Deadline = $"Deadline: {dto.Deadline}";
        StatusDisplay = dto.StatusDisplay;
        CanApply = dto.CanApply;
        HasApplied = dto.HasApplied;
        IsAwarded = dto.IsAwarded;
    }
}

public class CityGiftsViewModel : BaseViewModel
{
    private readonly CityGiftService _cityGiftService;
    private readonly NeighborhoodService _neighborhoodService;
    private readonly long _coordinatorId;

    private ObservableCollection<NeighborhoodDto> _neighborhoods = new();
    private NeighborhoodDto? _selectedNeighborhood;
    private ObservableCollection<CityGiftItemViewModel> _gifts = new();
    private bool _hasGifts;

    public CityGiftsViewModel(CityGiftService cityGiftService,
        NeighborhoodService neighborhoodService, long coordinatorId)
    {
        _cityGiftService = cityGiftService;
        _neighborhoodService = neighborhoodService;
        _coordinatorId = coordinatorId;
        LoadNeighborhoods();
    }

    public ObservableCollection<NeighborhoodDto> Neighborhoods
    {
        get => _neighborhoods;
        private set => SetProperty(ref _neighborhoods, value);
    }

    public NeighborhoodDto? SelectedNeighborhood
    {
        get => _selectedNeighborhood;
        set
        {
            SetProperty(ref _selectedNeighborhood, value);
            if (value != null) LoadGifts(value.Id);
        }
    }

    public ObservableCollection<CityGiftItemViewModel> Gifts
    {
        get => _gifts;
        private set => SetProperty(ref _gifts, value);
    }

    public bool HasGifts
    {
        get => _hasGifts;
        private set => SetProperty(ref _hasGifts, value);
    }

    public (bool success, string? error) Apply(long cityGiftId)
    {
        if (SelectedNeighborhood == null) return (false, "Please select a neighborhood.");
        var result = _cityGiftService.Apply(cityGiftId, _coordinatorId, SelectedNeighborhood.Id);
        if (result.success) LoadGifts(SelectedNeighborhood.Id);
        return result;
    }

    private void LoadNeighborhoods()
    {
        var neighborhoods = _neighborhoodService.GetByCoordinator(_coordinatorId);
        Neighborhoods = new ObservableCollection<NeighborhoodDto>(neighborhoods);
        if (Neighborhoods.Count > 0)
            SelectedNeighborhood = Neighborhoods[0];
    }

    private void LoadGifts(long neighborhoodId)
    {
        var gifts = _cityGiftService.GetAll(_coordinatorId, neighborhoodId);
        Gifts = new ObservableCollection<CityGiftItemViewModel>(
            gifts.Select((g, i) => new CityGiftItemViewModel(g, i + 1)));
        HasGifts = gifts.Count > 0;
    }
}