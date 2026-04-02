using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class RequestAccessDialog : Window
{
    private readonly Building _building;
    private readonly User _user;
    private readonly BuildingAccessRequestDbRepository _requestRepository;

    public RequestAccessDialog(Building building, User user)
    {
        InitializeComponent();
        _building = building;
        _user = user;
        _requestRepository = new BuildingAccessRequestDbRepository();

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

        bool isOccupied = _requestRepository.IsUnitOccupied(_building.Id, unitNumber);
        if (isOccupied)
        {
            WarningTextBlock.Text = $"Warning: Apartment {unitNumber} is already occupied by another user.";
            WarningPanel.Visibility = Visibility.Visible;
        }
        else
        {
            WarningPanel.Visibility = Visibility.Collapsed;
        }
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

        _requestRepository.Create(_user.Id, _building.Id, unitNumber);
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}