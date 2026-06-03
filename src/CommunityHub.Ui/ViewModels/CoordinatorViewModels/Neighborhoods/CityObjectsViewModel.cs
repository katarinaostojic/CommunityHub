using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CityObjectsViewModel : BaseViewModel
{
    private readonly CityObjectService _cityObjectService;
    private readonly long _neighborhoodId;

    private ObservableCollection<CityObjectItemViewModel> _cityObjects = new();
    private CityObjectItemViewModel? _selectedCityObject;
    private string _durationDaysText = "1";
    private DateTime _rangeFrom = DateTime.Today;
    private DateTime _rangeTo = DateTime.Today.AddDays(30);
    private SlotSuggestion? _foundSlot;
    private List<SlotSuggestion> _alternativeSlots = new();
    private SlotSuggestion? _selectedSlot;
    private bool _showNoSlotWarning;
    private bool _isObjectSelected;
    private bool _hasFoundSlot;
    private bool _hasAlternatives;
    private bool _canConfirm;

    public CityObjectsViewModel(CityObjectService cityObjectService, long neighborhoodId)
    {
        _cityObjectService = cityObjectService;
        _neighborhoodId = neighborhoodId;
        LoadCityObjects();
    }

    public ObservableCollection<CityObjectItemViewModel> CityObjects
    {
        get => _cityObjects;
        private set => SetProperty(ref _cityObjects, value);
    }

    public CityObjectItemViewModel? SelectedCityObject
    {
        get => _selectedCityObject;
        set
        {
            SetProperty(ref _selectedCityObject, value);
            IsObjectSelected = value != null;
            ResetSearchState();
        }
    }

    public string DurationDaysText
    {
        get => _durationDaysText;
        set => SetProperty(ref _durationDaysText, value);
    }

    public DateTime RangeFrom
    {
        get => _rangeFrom;
        set => SetProperty(ref _rangeFrom, value);
    }

    public DateTime RangeTo
    {
        get => _rangeTo;
        set => SetProperty(ref _rangeTo, value);
    }

    public SlotSuggestion? FoundSlot
    {
        get => _foundSlot;
        private set => SetProperty(ref _foundSlot, value);
    }

    public List<SlotSuggestion> AlternativeSlots
    {
        get => _alternativeSlots;
        private set => SetProperty(ref _alternativeSlots, value);
    }

    public SlotSuggestion? SelectedSlot
    {
        get => _selectedSlot;
        set
        {
            SetProperty(ref _selectedSlot, value);
            CanConfirm = value != null;
        }
    }

    public bool ShowNoSlotWarning
    {
        get => _showNoSlotWarning;
        private set => SetProperty(ref _showNoSlotWarning, value);
    }

    public bool IsObjectSelected
    {
        get => _isObjectSelected;
        private set => SetProperty(ref _isObjectSelected, value);
    }

    public bool HasFoundSlot
    {
        get => _hasFoundSlot;
        private set => SetProperty(ref _hasFoundSlot, value);
    }

    public bool HasAlternatives
    {
        get => _hasAlternatives;
        private set => SetProperty(ref _hasAlternatives, value);
    }

    public bool CanConfirm
    {
        get => _canConfirm;
        private set => SetProperty(ref _canConfirm, value);
    }

    public string? SearchForSlot()
    {
        if (SelectedCityObject == null)
            return "Please select a city object.";

        if (!int.TryParse(DurationDaysText, out int duration) || duration < 1)
            return "Duration must be a positive whole number.";

        DateOnly rangeFrom = DateOnly.FromDateTime(RangeFrom);
        DateOnly rangeTo = DateOnly.FromDateTime(RangeTo);

        if (rangeFrom >= rangeTo)
            return "Start date must be before end date.";

        if (duration > rangeTo.DayNumber - rangeFrom.DayNumber + 1)
            return "Duration cannot exceed the selected date range.";

        ReserveRequest request = new(
            SelectedCityObject.Id, _neighborhoodId,
            duration, rangeFrom, rangeTo);

        SlotSuggestion? slot = _cityObjectService.FindSlot(request);

        FoundSlot = slot;
        HasFoundSlot = slot != null;
        ShowNoSlotWarning = false;
        AlternativeSlots = new List<SlotSuggestion>();
        HasAlternatives = false;
        SelectedSlot = slot; // auto-select ako postoji

        if (slot != null)
            return null;

        // Nema u opsegu — traži alternative
        List<SlotSuggestion> alternatives = _cityObjectService.FindAlternativeSlots(request);
        AlternativeSlots = alternatives;
        HasAlternatives = alternatives.Count > 0;
        ShowNoSlotWarning = true;
        CanConfirm = false;
        return null;
    }

    public void ConfirmReservation()
    {
        if (SelectedCityObject == null || SelectedSlot == null) return;

        _cityObjectService.Reserve(
            SelectedCityObject.Id, _neighborhoodId,
            SelectedSlot.DateFrom, SelectedSlot.DateTo);

        LoadCityObjects();
        ResetSearchState();
    }

    private void LoadCityObjects()
    {
        CityObjects = new ObservableCollection<CityObjectItemViewModel>(
            _cityObjectService.GetAll()
                .Select(dto => new CityObjectItemViewModel(dto)));
    }

    private void ResetSearchState()
    {
        FoundSlot = null;
        HasFoundSlot = false;
        AlternativeSlots = new List<SlotSuggestion>();
        HasAlternatives = false;
        SelectedSlot = null;
        CanConfirm = false;
        ShowNoSlotWarning = false;
    }
}
