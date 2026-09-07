using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Budget;
using CommunityHub.Ui.Helpers.Citizen;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class DonateDialog : Window
{
    private readonly BudgetViewModel _viewModel;
    private List<DonationCategory> _categories = new();
    private bool _userInteracted = false;

    public DonateDialog(BudgetViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        LoadCategories();
        DonationDatePicker.SelectedDate = DateTime.Today;
    }

    private void LoadCategories()
    {
        _categories = _viewModel.GetCategories();
        CategoryComboBox.ItemsSource = _categories.Select(c => c.Name).ToList();
        if (_categories.Any())
            CategoryComboBox.SelectedIndex = 0;
    }

    private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _userInteracted = true;
        ValidateAmount(showError: true);
        UpdateDonateButtonState();
        HighlightField(AmountTextBox, ValidateAmount(showError: false));
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_userInteracted) return;
        bool ok = CategoryComboBox.SelectedIndex >= 0;
        CategoryErrorText.Text = ok ? "" : ResourceHelper.Get("Validate_SelectCategory", "⚠ Please select a category.");
        CategoryErrorText.Visibility = ok ? Visibility.Collapsed : Visibility.Visible;
        UpdateDonateButtonState();
    }

    private void DonationDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_userInteracted) return;
        ValidateDate(showError: true);
        UpdateDonateButtonState();
    }

    private bool ValidateAmount(bool showError)
    {
        string text = AmountTextBox?.Text ?? "";
        if (string.IsNullOrWhiteSpace(text))
        {
            if (showError) ShowAmountError(ResourceHelper.Get("Validate_Required", "⚠ Cannot be empty."));
            return false;
        }
        if (!decimal.TryParse(text.Replace(",", "."),
            NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
        {
            if (showError) ShowAmountError(ResourceHelper.Get("Validate_InvalidAmount", "⚠ Enter a valid number (e.g. 500 or 1500.50)."));
            return false;
        }
        if (amount <= 0)
        {
            if (showError) ShowAmountError(ResourceHelper.Get("Validate_PositiveNumber", "⚠ Enter a positive number."));
            return false;
        }
        if (amount > 10_000_000)
        {
            if (showError) ShowAmountError(ResourceHelper.Get("Validate_AmountTooLarge", "⚠ Amount too large (max 10,000,000 RSD)."));
            return false;
        }
        if (showError) HideAmountError();
        return true;
    }

    private bool ValidateDate(bool showError)
    {
        if (DonationDatePicker?.SelectedDate == null)
        {
            if (showError)
            {
                DateErrorText.Text = ResourceHelper.Get("Validate_SelectDate", "⚠ Please select a date.");
                DateErrorText.Visibility = Visibility.Visible;
            }
            return false;
        }
        if (DonationDatePicker.SelectedDate.Value.Date < DateTime.Today)
        {
            if (showError)
            {
                DateErrorText.Text = ResourceHelper.Get("Validate_PastDate", "⚠ Date cannot be in the past.");
                DateErrorText.Visibility = Visibility.Visible;
            }
            return false;
        }
        if (showError) DateErrorText.Visibility = Visibility.Collapsed;
        return true;
    }

    private bool ValidateAll()
    {
        bool amountOk = ValidateAmount(showError: true);
        bool categoryOk = CategoryComboBox.SelectedIndex >= 0;
        bool dateOk = ValidateDate(showError: true);

        if (!categoryOk)
        {
            CategoryErrorText.Text = ResourceHelper.Get("Validate_SelectCategory", "⚠ Please select a category.");
            CategoryErrorText.Visibility = Visibility.Visible;
        }

        return amountOk && categoryOk && dateOk;
    }

    private void UpdateDonateButtonState()
    {
        bool allValid = ValidateAmount(showError: false)
            && CategoryComboBox.SelectedIndex >= 0
            && ValidateDate(showError: false);
        DonateButton.Opacity = allValid ? 1.0 : 0.5;
    }

    private void ShowAmountError(string msg)
    {
        if (AmountErrorText == null) return;
        AmountErrorText.Text = msg;
        AmountErrorText.Visibility = Visibility.Visible;
    }

    private void HideAmountError()
    {
        if (AmountErrorText == null) return;
        AmountErrorText.Visibility = Visibility.Collapsed;
    }

    private static void HighlightField(TextBox tb, bool isValid)
    {
        tb.BorderBrush = isValid
            ? new SolidColorBrush(Color.FromRgb(0x43, 0xA0, 0x47))
            : new SolidColorBrush(Color.FromRgb(0xE5, 0x39, 0x35));
        tb.BorderThickness = new Thickness(2);
    }

    private void DonateButton_Click(object sender, RoutedEventArgs e)
    {
        _userInteracted = true;
        if (!ValidateAll()) return;

        decimal amount = decimal.Parse(
            AmountTextBox.Text.Replace(",", "."),
            NumberStyles.Any, CultureInfo.InvariantCulture);
        long categoryId = _categories[CategoryComboBox.SelectedIndex].Id;

        var (success, error) = _viewModel.Donate(categoryId, amount);

        if (!success)
        {
            ShowAmountError($"⚠ {error}");
            return;
        }

        MsgHelper.Info("Msg_DonationSuccess", "Msg_Success");
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
