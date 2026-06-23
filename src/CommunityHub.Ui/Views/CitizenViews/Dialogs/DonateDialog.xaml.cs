using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Budget;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class DonateDialog : Window
{
    private readonly BudgetViewModel _viewModel;
    private List<DonationCategory> _categories = new();
    private bool _userInteracted = false; // live validacija se aktivira tek kad korisnik počne da piše

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
        CategoryErrorText.Text = ok ? "" : "⚠ Molimo odaberite kategoriju.";
        CategoryErrorText.Visibility = ok ? Visibility.Collapsed : Visibility.Visible;
        UpdateDonateButtonState();
    }

    private void DonationDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_userInteracted) return;
        bool ok = ValidateDate(showError: true);
        UpdateDonateButtonState();
    }

    // Vraća true ako je iznos validan
    private bool ValidateAmount(bool showError)
    {
        string text = AmountTextBox?.Text ?? "";
        if (string.IsNullOrWhiteSpace(text))
        {
            if (showError) ShowAmountError("⚠ Iznos ne sme biti prazan.");
            return false;
        }
        if (!decimal.TryParse(text.Replace(",", "."),
            NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
        {
            if (showError) ShowAmountError("⚠ Unesite ispravan decimalni broj (npr. 500 ili 1500.50).");
            return false;
        }
        if (amount <= 0)
        {
            if (showError) ShowAmountError("⚠ Iznos mora biti veći od 0.");
            return false;
        }
        if (amount > 10_000_000)
        {
            if (showError) ShowAmountError("⚠ Iznos je prevelik (max 10,000,000 RSD).");
            return false;
        }
        if (showError) HideAmountError();
        return true;
    }

    private bool ValidateDate(bool showError)
    {
        bool ok = DonationDatePicker?.SelectedDate != null;
        if (showError)
        {
            DateErrorText.Text = ok ? "" : "⚠ Molimo odaberite datum.";
            DateErrorText.Visibility = ok ? Visibility.Collapsed : Visibility.Visible;
        }
        return ok;
    }

    private bool ValidateAll()
    {
        bool amountOk = ValidateAmount(showError: true);
        bool categoryOk = CategoryComboBox.SelectedIndex >= 0;
        bool dateOk = ValidateDate(showError: true);

        if (!categoryOk)
        {
            CategoryErrorText.Text = "⚠ Molimo odaberite kategoriju.";
            CategoryErrorText.Visibility = Visibility.Visible;
        }

        return amountOk && categoryOk && dateOk;
    }

    private void UpdateDonateButtonState()
    {
        bool allValid = ValidateAmount(showError: false)
            && CategoryComboBox.SelectedIndex >= 0
            && DonationDatePicker?.SelectedDate != null;
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
            ? new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x43, 0xA0, 0x47)) // zelena
            : new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE5, 0x39, 0x35)); // crvena
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
