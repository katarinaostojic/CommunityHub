using CommunityHub.Application.DTOs.Buildings;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ResidentMeetings;

public class ResidentMeetingApartmentSelector : BaseViewModel
{
    private string _selectedUnitNumber;

    public ResidentMeetingApartmentSelector(
        IEnumerable<BuildingMembershipDto> memberships,
        BuildingMembershipDto selectedMembership)
    {
        UnitNumbers = new ObservableCollection<string>(
            memberships
                .Select(membership => membership.UnitNumber)
                .Distinct()
                .OrderBy(unitNumber => int.TryParse(unitNumber, out int number) ? number : int.MaxValue)
                .ToList());

        _selectedUnitNumber = selectedMembership.UnitNumber;
    }

    public event Action? SelectedApartmentChanged;

    public ObservableCollection<string> UnitNumbers { get; }

    public string SelectedUnitNumber
    {
        get => _selectedUnitNumber;
        set
        {
            if (!SetProperty(ref _selectedUnitNumber, value))
                return;

            OnPropertyChanged(nameof(SelectedApartmentDisplay));
            SelectedApartmentChanged?.Invoke();
        }
    }

    public string SelectedApartmentDisplay => $"Apartment {SelectedUnitNumber}";

    public bool ShowSelector => UnitNumbers.Count > 1;
}