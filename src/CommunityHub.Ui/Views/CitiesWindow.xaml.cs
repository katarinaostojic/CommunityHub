using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views
{
    public partial class CitiesWindow : Window
    {
        private readonly CityDbRepository _cityRepository;
        private readonly CountryDbRepository _countryRepository;
        private City _selectedCity;

        public CitiesWindow()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _cityRepository = new CityDbRepository();
                _countryRepository = new CountryDbRepository();
                LoadCities();
                LoadCountries();
            }
        }

        private void LoadCities()
        {
            List<City> cities = _cityRepository.GetAll();
            CitiesDataGrid.ItemsSource = cities;
        }

        private void LoadCountries()
        {
            List<Country> countries = _countryRepository.GetAll();
            CountryComboBox.ItemsSource = countries;
        }

        private void CitiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCity = CitiesDataGrid.SelectedItem as City;

            if (_selectedCity != null)
            {
                CityNameTextBox.Text = _selectedCity.Name;
                CountryComboBox.SelectedItem = CountryComboBox.Items
                    .Cast<Country>()
                    .FirstOrDefault(c => c.Id == _selectedCity.Country.Id);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = CityNameTextBox.Text.Trim();
            Country selectedCountry = CountryComboBox.SelectedItem as Country;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a city name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedCountry == null)
            {
                MessageBox.Show("Please select a country.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            City newCity = new City(name, selectedCountry);
            _cityRepository.Create(newCity);
            LoadCities();
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCity == null)
            {
                MessageBox.Show("Please select a city to update.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = CityNameTextBox.Text.Trim();
            Country selectedCountry = CountryComboBox.SelectedItem as Country;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a city name.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedCountry == null)
            {
                MessageBox.Show("Please select a country.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            City updatedCity = new City(_selectedCity.Id, name, selectedCountry);
            _cityRepository.Update(updatedCity);
            LoadCities();
            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCity == null)
            {
                MessageBox.Show("Please select a city to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the city '{_selectedCity.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _cityRepository.Delete(_selectedCity.Id);
                LoadCities();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            CityNameTextBox.Text = string.Empty;
            CountryComboBox.SelectedItem = null;
            CitiesDataGrid.SelectedItem = null;
            _selectedCity = null;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}