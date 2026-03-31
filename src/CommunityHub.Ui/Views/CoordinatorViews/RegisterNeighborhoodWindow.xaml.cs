using System.Windows;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class RegisterNeighborhoodWindow : Window
{
    private readonly long _coordinatorId;
    private readonly NeighborhoodDbRepository _neighborhoodRepository = new();
    private readonly CityDbRepository _cityRepository = new();
    private readonly List<Street> _streets = new();

    public RegisterNeighborhoodWindow(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        LoadCities();
    }

    private void LoadCities()
    {
        var cities = _cityRepository.GetAll();
        CityComboBox.ItemsSource = cities;
    }

    private void AddStreet_Click(object sender, RoutedEventArgs e)
    {
        string streetName = StreetNameTextBox.Text.Trim();
        string startText = StartNumberTextBox.Text.Trim();
        string endText = EndNumberTextBox.Text.Trim();

        if (string.IsNullOrEmpty(streetName) || string.IsNullOrEmpty(startText) || string.IsNullOrEmpty(endText))
        {
            MessageBox.Show("Please fill in all street fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!int.TryParse(startText, out int startNumber) || !int.TryParse(endText, out int endNumber))
        {
            MessageBox.Show("Start and end numbers must be integers.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (startNumber >= endNumber)
        {
            MessageBox.Show("Start number must be less than end number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Street street = new Street(0, 0, streetName, startNumber, endNumber);
        _streets.Add(street);
        StreetsListBox.Items.Add(street.ToString());

        StreetNameTextBox.Clear();
        StartNumberTextBox.Clear();
        EndNumberTextBox.Clear();
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameTextBox.Text.Trim();
        string description = DescriptionTextBox.Text.Trim();
        City? selectedCity = CityComboBox.SelectedItem as City;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
        {
            MessageBox.Show("Name and description are required.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (selectedCity == null)
        {
            MessageBox.Show("Please select a city.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (_streets.Count == 0)
        {
            MessageBox.Show("Please add at least one street.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        long neighborhoodId = _neighborhoodRepository.Create(name, description, selectedCity.Id, _coordinatorId);

        foreach (Street street in _streets)
        {
            _neighborhoodRepository.AddStreet(neighborhoodId, street.StreetName, street.StartNumber, street.EndNumber);
        }

        MessageBox.Show("Neighborhood registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        this.Close();
    }
}