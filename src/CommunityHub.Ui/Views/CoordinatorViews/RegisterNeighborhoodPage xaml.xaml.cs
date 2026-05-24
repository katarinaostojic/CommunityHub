using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class RegisterNeighborhoodPage : Page
{
    private readonly long _coordinatorId;
    private readonly NeighborhoodService _neighborhoodService;
    private readonly CityService _cityService;
    private readonly List<Street> _streets = new();
    private readonly List<string> _imagePaths = new();
    private int _currentImageIndex = 0;

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

    private void ShowError(string message)
    {
        ErrorText.Text = message;
        ErrorBanner.Visibility = Visibility.Visible;
        SuccessBanner.Visibility = Visibility.Collapsed;
    }

    private void ShowSuccess(string message)
    {
        SuccessText.Text = message;
        SuccessBanner.Visibility = Visibility.Visible;
        ErrorBanner.Visibility = Visibility.Collapsed;
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
        ErrorBanner.Visibility = Visibility.Collapsed;
    }

    private bool ValidateStreetInputs(string streetName, string startText, string endText, out int startNumber, out int endNumber)
    {
        startNumber = 0;
        endNumber = 0;

        if (string.IsNullOrEmpty(streetName) || string.IsNullOrEmpty(startText) || string.IsNullOrEmpty(endText))
        {
            ShowError("Please fill in all street fields.");
            return false;
        }

        if (!int.TryParse(startText, out startNumber) || !int.TryParse(endText, out endNumber))
        {
            ShowError("Start and end numbers must be integers.");
            return false;
        }

        if (startNumber >= endNumber)
        {
            ShowError("Start number must be less than end number.");
            return false;
        }

        return true;
    }

    private void RemoveStreet_Click(object sender, RoutedEventArgs e)
    {
        int selectedIndex = StreetsListBox.SelectedIndex;
        if (selectedIndex < 0)
        {
            ShowError("Please select a street to remove.");
            return;
        }
        _streets.RemoveAt(selectedIndex);
        StreetsListBox.Items.RemoveAt(selectedIndex);
        ErrorBanner.Visibility = Visibility.Collapsed;
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
                    _imagePaths.Add(path);
            }
            RefreshImagesPreview();
        }
    }

    private void RemoveImage_Click(object sender, RoutedEventArgs e)
    {
        if (_imagePaths.Count == 0) return;
        _imagePaths.RemoveAt(_currentImageIndex);
        RefreshImagesPreview();
    }

    private void PrevImage_Click(object sender, RoutedEventArgs e)
    {
        if (_imagePaths.Count == 0) return;
        _currentImageIndex = (_currentImageIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
        RefreshImagesPreview();
    }

    private void NextImage_Click(object sender, RoutedEventArgs e)
    {
        if (_imagePaths.Count == 0) return;
        _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Count;
        RefreshImagesPreview();
    }

    private void RefreshImagesPreview()
    {
        if (_imagePaths.Count == 0)
        {
            PreviewImage.Source = null;
            RemoveImageButton.Visibility = Visibility.Collapsed;
            PrevButton.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
            return;
        }
        if (_currentImageIndex >= _imagePaths.Count)
            _currentImageIndex = _imagePaths.Count - 1;
        PreviewImage.Source = new System.Windows.Media.Imaging.BitmapImage(
            new Uri(_imagePaths[_currentImageIndex]));
        RemoveImageButton.Visibility = Visibility.Visible;
        PrevButton.Visibility = _imagePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
        NextButton.Visibility = _imagePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
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

        ShowSuccess("Neighborhood registered successfully!");

        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_coordinatorId), "My Districts");
    }

    private bool ValidateInputs(string name, string description, City? selectedCity)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
        {
            ShowError("Name and description are required.");
            return false;
        }

        if (selectedCity == null)
        {
            ShowError("Please select a city.");
            return false;
        }

        if (_streets.Count == 0)
        {
            ShowError("Please add at least one street.");
            return false;
        }

        if (_imagePaths.Count == 0)
        {
            ShowError("Please add at least one image.");
            return false;
        }

        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_coordinatorId), "My Districts");
    }
}