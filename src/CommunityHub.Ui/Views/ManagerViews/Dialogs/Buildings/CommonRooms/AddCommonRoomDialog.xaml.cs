using CommunityHub.Application.Domain.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Helpers.Manager;
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

        TooltipsManager.Apply(this);
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
            ShowFieldError(NameErrorText, ex.Message);
        }
    }

    private bool ValidateFields()
    {
        ClearFieldErrors();
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            ShowFieldError(NameErrorText, "Name is required.");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
        {
            ShowFieldError(DescriptionErrorText, "Description is required.");
            isValid = false;
        }

        if (!int.TryParse(FloorTextBox.Text.Trim(), out _))
        {
            ShowFieldError(FloorErrorText, "Please enter a valid floor number.");
            isValid = false;
        }

        if (RentalTypeComboBox.SelectedItem == null)
        {
            ShowFieldError(RentalTypeErrorText, "Please select a rental type.");
            isValid = false;
        }

        return isValid;
    }

    private void ShowFieldError(TextBlock errorText, string message)
    {
        errorText.Text = message;
        errorText.Visibility = Visibility.Visible;
    }

    private void HideFieldError(TextBlock errorText)
    {
        errorText.Visibility = Visibility.Collapsed;
    }

    private void ClearFieldErrors()
    {
        HideFieldError(NameErrorText);
        HideFieldError(DescriptionErrorText);
        HideFieldError(FloorErrorText);
        HideFieldError(RentalTypeErrorText);
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;
        DialogResult = false;
        Close();
    }
}