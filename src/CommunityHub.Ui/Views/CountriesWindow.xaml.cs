using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views
{
    public partial class CountriesWindow : Window
    {
        private readonly CountryDbRepository _countryRepository;
        private readonly HomeWindow _homeWindow;
        private Country _selectedCountry;

        public CountriesWindow(HomeWindow homeWindow)
        {
            InitializeComponent();
            _homeWindow = homeWindow;

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _countryRepository = new CountryDbRepository();
                LoadCountries();
            }
        }

        private void LoadCountries()
        {
            List<Country> countries = _countryRepository.GetAll();
            CountriesDataGrid.ItemsSource = countries;
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
                MessageBox.Show("Unesite naziv države.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Unesite kod države.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Country newCountry = new Country(name, code);
            _countryRepository.Create(newCountry);
            LoadCountries();
            ClearForm();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCountry == null)
            {
                MessageBox.Show("Izaberite državu za izmenu.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string name = CountryNameTextBox.Text.Trim();
            string code = CountryCodeTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Unesite naziv države.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Unesite kod države.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Country updatedCountry = new Country(_selectedCountry.Id, name, code);
            _countryRepository.Update(updatedCountry);
            LoadCountries();
            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCountry == null)
            {
                MessageBox.Show("Izaberite državu za brisanje.", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Da li ste sigurni da zelite da obrisete državu '{_selectedCountry.Name}'?",
                "Potvrda brisanja",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                bool deleted = _countryRepository.Delete(_selectedCountry.Id);

                if (!deleted)
                {
                    MessageBox.Show(
                        $"Nije moguće obrisati državu '{_selectedCountry.Name}' jer ima povezanih gradova.",
                        "Brisanje nije moguće",
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