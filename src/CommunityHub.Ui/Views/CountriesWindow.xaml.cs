using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System.Collections.Generic;
using System.Windows;

namespace CommunityHub.Ui.Views
{
    public partial class CountriesWindow : Window
    {
        private readonly CountryDbRepository _countryRepository;

        public CountriesWindow()
        {
            InitializeComponent();
            _countryRepository = new CountryDbRepository();
            LoadCountries();
        }

        private void LoadCountries()
        {
            List<Country> countries = _countryRepository.GetAll();
            CountriesDataGrid.ItemsSource = countries;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}