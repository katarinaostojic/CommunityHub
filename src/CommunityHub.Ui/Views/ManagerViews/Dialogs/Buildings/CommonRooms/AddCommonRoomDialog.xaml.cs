using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Controls;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class AddCommonRoomDialog : Window
{
    private readonly BuildingDto _building;
    private readonly BuildingDetailsViewModel _viewModel;
    private FloatingKeyboardWindow? _activeFloating;

    public AddCommonRoomDialog(User currentUser, BuildingDto building, BuildingDetailsViewModel viewModel)
    {
        InitializeComponent();
        _building = building;
        _viewModel = viewModel;

        Loaded += (s, e) =>
        {
            NameTextBox.PreviewMouseDown += (s2, e2) => OpenKeyboard(NameTextBox, "Name");
            DescriptionTextBox.PreviewMouseDown += (s2, e2) => OpenKeyboard(DescriptionTextBox, "Description");
            FloorTextBox.PreviewMouseDown += (s2, e2) => OpenKeyboard(FloorTextBox, "Floor");
        };
    }

    private void OpenKeyboard(TextBox textBox, string fieldName)
    {
        _activeFloating?.Close();
        _activeFloating = new FloatingKeyboardWindow(textBox, this, fieldName);
        _activeFloating.Closed += (_, _) => _activeFloating = null;
        _activeFloating.Show();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;

        if (!ValidateFields()) return;

        try
        {
            int floorNumber = int.Parse(FloorTextBox.Text.Trim());
            RentalType rentalType = RentalTypeComboBox.SelectedIndex == 0
                ? RentalType.PerDay
                : RentalType.MultiDay;

            _viewModel.CreateCommonRoom(
                NameTextBox.Text.Trim(),
                DescriptionTextBox.Text.Trim(),
                floorNumber,
                rentalType
            );

            MessageBox.Show("A new common room has been added successfully.",
                "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool ValidateFields()
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show("Please enter a name for the common room.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            MessageBox.Show("Please enter a description.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!int.TryParse(FloorTextBox.Text.Trim(), out _))
        {
            MessageBox.Show("Please enter a valid floor number.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (RentalTypeComboBox.SelectedItem == null)
        {
            MessageBox.Show("Please select a rental type.",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;
        DialogResult = false;
        Close();
    }
}