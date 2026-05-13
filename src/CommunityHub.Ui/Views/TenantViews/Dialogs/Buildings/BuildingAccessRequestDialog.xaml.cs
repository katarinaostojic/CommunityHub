using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BuildingAccessRequestDialog : Window
{
    private readonly BuildingAccessRequestDialogViewModel _viewModel;

    public BuildingAccessRequestDialog(Building building, User user)
    {
        InitializeComponent();

        BuildingAccessRequestService requestService = Injector.CreateInstance<BuildingAccessRequestService>();
        _viewModel = new BuildingAccessRequestDialogViewModel(requestService, building, user);

        TitleTextBlock.Text = $"REQUEST ACCESS: {building.Street} {building.StreetNumber}";
        BuildingInfoTextBlock.Text = $"Building: {building.Street} {building.StreetNumber}, {building.City.Name}, {building.Neighborhood}";

        UnitComboBox.ItemsSource = _viewModel.SortedUnitNumbers;
    }

    private void CheckUnitOccupied()
    {
        string unitNumber = UnitComboBox.Text.Trim();
        if (string.IsNullOrEmpty(unitNumber))
        {
            WarningPanel.Visibility = Visibility.Collapsed;
            return;
        }

        bool isOccupied = _viewModel.IsUnitOccupied(unitNumber);
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

        _viewModel.SubmitRequest(unitNumber);
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
        if (!_viewModel.ContainsUnit(unitNumber))
        {
            MessageBox.Show("Please select a valid apartment number from the list.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
        if (_viewModel.HasExistingRequest(unitNumber))
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