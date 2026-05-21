using CommunityHub.Application.DependencyInjection;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CommunityHub.Application.Services.Shared;
using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Ui.Views
{
    public partial class CountriesWindow : Window
    {
        private readonly CountryService _countryService;
        private readonly HomeWindow _homeWindow;
        private Country _selectedCountry;

        public CountriesWindow(HomeWindow homeWindow)
        {
            InitializeComponent();
            _homeWindow = homeWindow;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _countryService = Injector.CreateInstance<CountryService>();
                LoadCountries();
            }
        }

        private void LoadCountries()
        {
            CountriesDataGrid.ItemsSource = _countryService.GetAll();
        }

        private void CountriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCountry = CountriesDataGrid.SelectedItem as Country;

            if (_selectedCountry != null)
            {
                CountryNameTextBox.Text = _selectedCountry.Name;
                CountryCodeTextBox.Text = _selectedCountry.Code;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = CountryNameTextBox.Text.Trim();
            string code = CountryCodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a country name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please enter a country code.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _countryService.Create(new Country(name, code));
            LoadCountries();
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCountry == null)
            {
                MessageBox.Show("Please select a country to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = CountryNameTextBox.Text.Trim();
            string code = CountryCodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a country name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please enter a country code.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _countryService.Update(new Country(_selectedCountry.Id, name, code));
            LoadCountries();
            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCountry == null)
            {
                MessageBox.Show("Please select a country to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the country '{_selectedCountry.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool deleted = _countryService.Delete(_selectedCountry.Id);

                if (!deleted)
                {
                    MessageBox.Show(
                        $"Cannot delete country '{_selectedCountry.Name}' because it has associated cities.",
                        "Delete Not Allowed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                LoadCountries();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            CountryNameTextBox.Text = string.Empty;
            CountryCodeTextBox.Text = string.Empty;
            CountriesDataGrid.SelectedItem = null;
            _selectedCountry = null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _homeWindow.Show();
            this.Close();
        }
    }
}