using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Services.TenantServices;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class RequestAccessDialog : Window
{
    private readonly Building _building;
    private readonly User _user;
    private readonly BuildingAccessRequestService _requestService;

    public RequestAccessDialog(Building building, User user)
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

        bool isOccupied = _requestService.IsUnitOccupied(_building.Id, unitNumber);
        WarningTextBlock.Text = isOccupied
            ? $"Warning: Apartment {unitNumber} is already occupied by another user."
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

        if (string.IsNullOrEmpty(unitNumber))
        {
            MessageBox.Show("Please enter an apartment number.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!IsValidUnit(unitNumber))
        {
            MessageBox.Show("Please select a valid apartment number from the list.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _requestService.Create(_user.Id, _building.Id, unitNumber);
        DialogResult = true;
        Close();
    }

    private bool IsValidUnit(string unitNumber)
    {
        return _building.Floors
            .SelectMany(f => f.Units)
            .Any(u => u.UnitNumber == unitNumber);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}