using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Application.Domain.Entities.Neighborhoods.Budget;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class DonateDialog : Window
{
    private readonly BudgetViewModel _viewModel;
    private List<DonationCategory> _categories = new();

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

    private bool TryParseAmount(out decimal amount)
    {
        if (decimal.TryParse(AmountTextBox.Text.Replace(",", "."),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out amount))
            return true;

        MessageBox.Show("Unesite ispravan iznos.", "Greška",
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private bool TryGetSelectedCategoryId(out long categoryId)
    {
        categoryId = 0;
        int selectedIndex = CategoryComboBox.SelectedIndex;
        if (selectedIndex >= 0)
        {
            categoryId = _categories[selectedIndex].Id;
            return true;
        }

        MessageBox.Show("Odaberite kategoriju.", "Greška",
            MessageBoxButton.OK, MessageBoxImage.Warning);
        return false;
    }

    private void DonateButton_Click(object sender, RoutedEventArgs e)
    {
        if (!TryParseAmount(out decimal amount)) return;
        if (!TryGetSelectedCategoryId(out long categoryId)) return;

        var (success, error) = _viewModel.Donate(categoryId, amount);

        if (!success)
        {
            MessageBox.Show(error, "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBox.Show("Donacija je uspešno evidentirana.", "Uspeh",
            MessageBoxButton.OK, MessageBoxImage.Information);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
}
