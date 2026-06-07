using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ReserveCityObjectViewModel : BaseViewModel
{
    private readonly CityObjectService _cityObjectService;
    private readonly long _neighborhoodId;

    private string _durationDaysText = "1";
    private DateTime _rangeFrom = DateTime.Today;
    private DateTime _rangeTo = DateTime.Today.AddDays(30);
    private bool _rangeFromSelected = false;
    private bool _rangeToSelected = false;
    private SlotSuggestion? _foundSlot;
    private List<SlotSuggestion> _alternativeSlots = new();
    private SlotSuggestion? _selectedSlot;
    private bool _showNoSlotWarning;
    private bool _hasFoundSlot;
    private bool _hasAlternatives;
    private bool _canConfirm;

    public ReserveCityObjectViewModel(CityObjectItemViewModel cityObject,
        CityObjectService cityObjectService, long neighborhoodId)
    {
        SelectedCityObject = cityObject;
        _cityObjectService = cityObjectService;
        _neighborhoodId = neighborhoodId;
    }

    public CityObjectItemViewModel SelectedCityObject { get; }

    public string DurationDaysText
    {
        get => _durationDaysText;
        set => SetProperty(ref _durationDaysText, value);
    }

    public DateTime RangeFrom
    {
        get => _rangeFrom;
        set
        {
            SetProperty(ref _rangeFrom, value);
            _rangeFromSelected = true;
        }
    }

    public DateTime RangeTo
    {
        get => _rangeTo;
        set
        {
            SetProperty(ref _rangeTo, value);
            _rangeToSelected = true;
        }
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
        if (!int.TryParse(DurationDaysText, out int duration) || duration < 1)
            return "Duration must be a positive whole number.";

        if (!_rangeFromSelected)
            return "Please select a start date.";

        if (!_rangeToSelected)
            return "Please select an end date.";

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
        SelectedSlot = slot;

        if (slot != null)
            return null;

        List<SlotSuggestion> alternatives = _cityObjectService.FindAlternativeSlots(request);
        AlternativeSlots = alternatives;
        HasAlternatives = alternatives.Count > 0;
        ShowNoSlotWarning = true;
        CanConfirm = false;
        return null;
    }

    public void ConfirmReservation()
    {
        if (SelectedSlot == null) return;
        _cityObjectService.Reserve(
            SelectedCityObject.Id, _neighborhoodId,
            SelectedSlot.DateFrom, SelectedSlot.DateTo);
    }
}