using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Application.Services.Shared;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class RegisterNeighborhoodPage : Page
{
    private readonly long _coordinatorId;
    private readonly NeighborhoodService _neighborhoodService;
    private readonly CityService _cityService;
    private readonly List<Street> _streets = new();
    private readonly List<string> _imagePaths = new();

    public RegisterNeighborhoodPage(long coordinatorId)
    {
        InitializeComponent();
        _coordinatorId = coordinatorId;
        _neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        _cityService = Injector.CreateInstance<CityService>();
        LoadCities();
    }

    private void LoadCities()
    {
        var cities = _cityService.GetAll();
        CityComboBox.ItemsSource = cities;
    }

    private void AddStreet_Click(object sender, RoutedEventArgs e)
    {
        string streetName = StreetNameTextBox.Text.Trim();
        string startText = StartNumberTextBox.Text.Trim();
        string endText = EndNumberTextBox.Text.Trim();

        if (!ValidateStreetInputs(streetName, startText, endText, out int startNumber, out int endNumber))
            return;

        Street street = new Street(0, 0, streetName, startNumber, endNumber);
        _streets.Add(street);
        StreetsListBox.Items.Add(street.ToString());

        StreetNameTextBox.Clear();
        StartNumberTextBox.Clear();
        EndNumberTextBox.Clear();
    }

    private bool ValidateStreetInputs(string streetName, string startText, string endText, out int startNumber, out int endNumber)
    {
        startNumber = 0;
        endNumber = 0;

        if (string.IsNullOrEmpty(streetName) || string.IsNullOrEmpty(startText) || string.IsNullOrEmpty(endText))
        {
            MessageBox.Show("Please fill in all street fields.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (!int.TryParse(startText, out startNumber) || !int.TryParse(endText, out endNumber))
        {
            MessageBox.Show("Start and end numbers must be integers.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (startNumber >= endNumber)
        {
            MessageBox.Show("Start number must be less than end number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private void AddImage_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp";
        dialog.Multiselect = true;

        if (dialog.ShowDialog() == true)
        {
            foreach (string path in dialog.FileNames)
            {
                if (!_imagePaths.Contains(path))
                {
                    _imagePaths.Add(path);
                    ImagesListBox.Items.Add(path);
                }
            }
        }
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string name = NameTextBox.Text.Trim();
        string description = DescriptionTextBox.Text.Trim();
        City? selectedCity = CityComboBox.SelectedItem as City;

        if (!ValidateInputs(name, description, selectedCity))
            return;

        long neighborhoodId = _neighborhoodService.Create(name, description, selectedCity!.Id, _coordinatorId);

        foreach (Street street in _streets)
            _neighborhoodService.AddStreet(neighborhoodId, street.StreetName, street.StartNumber, street.EndNumber);

        foreach (string path in _imagePaths)
            _neighborhoodService.AddImage(neighborhoodId, path);

        MessageBox.Show("Neighborhood registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_coordinatorId), "My Districts");
    }

    private bool ValidateInputs(string name, string description, City? selectedCity)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
        {
            MessageBox.Show("Name and description are required.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (selectedCity == null)
        {
            MessageBox.Show("Please select a city.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (_streets.Count == 0)
        {
            MessageBox.Show("Please add at least one street.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (_imagePaths.Count == 0)
        {
            MessageBox.Show("Please add at least one image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_coordinatorId), "My Districts");
    }
}