using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Services;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BuildingAccessRequestDialog : Window
{
    private readonly Building _building;
    private readonly User _user;
    private readonly BuildingAccessRequestService _requestService;

    public BuildingAccessRequestDialog(Building building, User user)
    {
        InitializeComponent();
        _building = building;
        _user = user;
        _requestService = new BuildingAccessRequestService();

        TitleTextBlock.Text = $"REQUEST ACCESS: {building.Street} {building.StreetNumber}";
        BuildingInfoTextBlock.Text = $"Building: {building.Street} {building.StreetNumber}, {building.City.Name}, {building.Neighborhood}";

        UnitComboBox.ItemsSource = _building.Floors
            .SelectMany(f => f.Units)
            .Select(u => u.UnitNumber)
            .OrderBy(u => int.TryParse(u, out int n) ? n : int.MaxValue)
            .ToList();
    }

    private void CheckUnitOccupied()
    {
        string unitNumber = UnitComboBox.Text.Trim();
        if (string.IsNullOrEmpty(unitNumber))
        {
            WarningPanel.Visibility = Visibility.Collapsed;
            return;
        }

        bool isOccupied = _building.IsUnitOccupied(unitNumber);
        WarningTextBlock.Text = isOccupied
            ? $"Warning: Apartment {unitNumber} is already occupied by another user.\nYou can still submit a request."
            : string.Empty;
        WarningPanel.Visibility = isOccupied ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UnitComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CheckUnitOccupied();
    }

    private void UnitComboBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        CheckUnitOccupied();
    }

    private void SendRequestButton_Click(object sender, RoutedEventArgs e)
    {
        string unitNumber = UnitComboBox.Text.Trim();
        if (!ValidateUnitSelection(unitNumber)) return;

        _requestService.Create(_user, _building, unitNumber);
        DialogResult = true;
        Close();
    }

    private bool ValidateUnitSelection(string unitNumber)
    {
        if (string.IsNullOrEmpty(unitNumber))
        {
            MessageBox.Show("Please enter an apartment number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (!_building.ContainsUnit(unitNumber))
        {
            MessageBox.Show("Please select a valid apartment number from the list.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (_building.HasExistingRequest(_user.Id, unitNumber))
        {
            MessageBox.Show("You already have a request for this apartment.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}