using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Application.Services.Entities.Shared;
using CommunityHub.Ui.Helpers.Manager;
using CommunityHub.Ui.Views.ManagerViews.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class RegisterBuildingDialog : Window
{
    private readonly User _currentUser;
    private readonly BuildingService _buildingService;
    private readonly CityService _cityService;
    private readonly CountryService _countryService;
    private List<string> _selectedImagePaths = new List<string>();
    private bool _updatingFromCity = false;
    private FloatingKeyboardWindow? _activeFloating;

    public RegisterBuildingDialog(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _buildingService = Injector.CreateInstance<BuildingService>();
        _cityService = Injector.CreateInstance<CityService>();
        _countryService = Injector.CreateInstance<CountryService>();
        LoadCountries();
        LoadCities();

        StreetTextBox.PreviewMouseDown += (s, e) => OpenKeyboard(StreetTextBox, "Street");
        NumberTextBox.PreviewMouseDown += (s, e) => OpenKeyboard(NumberTextBox, "Number");
        SettlementTextBox.PreviewMouseDown += (s, e) => OpenKeyboard(SettlementTextBox, "Settlement");
        FloorsTextBox.PreviewMouseDown += (s, e) => OpenKeyboard(FloorsTextBox, "Number of Floors");

        TooltipsManager.Apply(this);
    }

    private void OpenKeyboard(TextBox textBox, string fieldName)
    {
        _activeFloating?.Close();
        _activeFloating = new FloatingKeyboardWindow(textBox, this, fieldName);
        _activeFloating.Closed += (_, _) => _activeFloating = null;
        _activeFloating.Show();
    }

    private void LoadCountries()
    {
        var countries = _countryService.GetAll();
        CountryComboBox.ItemsSource = countries;
        CountryComboBox.DisplayMemberPath = "Name";
    }

    private void LoadCities(long? countryId = null)
    {
        var cities = countryId.HasValue
            ? _cityService.GetByCountry(countryId.Value)
            : _cityService.GetAll();

        CityComboBox.ItemsSource = cities;
        CityComboBox.DisplayMemberPath = "Name";
        CityComboBox.SelectedItem = null;
    }

    private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_updatingFromCity) return;

        if (CountryComboBox.SelectedItem is Country country)
            LoadCities(country.Id);
        else
            LoadCities();
    }

    private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CityComboBox.SelectedItem is City city)
        {
            _updatingFromCity = true;
            CountryComboBox.SelectedItem = CountryComboBox.Items
                .Cast<Country>()
                .FirstOrDefault(c => c.Id == city.Country.Id);
            _updatingFromCity = false;
        }
    }

    private void ConfirmFloors_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;

        FloorsStackPanel.Children.Clear();

        if (!int.TryParse(FloorsTextBox.Text, out int numberOfFloors) || numberOfFloors <= 0)
        {
            ShowFieldError(FloorsErrorText, "Please enter a valid number of floors.");
            return;
        }

        HideFieldError(FloorsErrorText);

        for (int i = 1; i <= numberOfFloors; i++)
            FloorsStackPanel.Children.Add(CreateFloorRow(i));

        TooltipsManager.Apply(this);

        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, () =>
        {
            double neededHeight = 650 + (numberOfFloors * 55) + 100;
            double maxHeight = SystemParameters.WorkArea.Height - 50;
            Height = Math.Min(neededHeight, maxHeight);
            Top = (SystemParameters.WorkArea.Height - Height) / 2;
        });
    }

    private Grid CreateFloorRow(int floorNumber)
    {
        Grid floorGrid = new Grid();
        floorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
        floorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        floorGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        floorGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        floorGrid.Margin = new Thickness(0, 10, 0, 10);

        TextBlock label = new TextBlock
        {
            Text = $"Floor {floorNumber} units (e.g. 1,2,3) *",
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14
        };

        TextBox textBox = new TextBox
        {
            Height = 35,
            FontSize = 14,
            Tag = floorNumber,
            ToolTip = "List unit numbers separated by commas, e.g. 1,2,3"
        };
        textBox.PreviewMouseDown += (s, e) => OpenKeyboard(textBox, $"Floor {floorNumber} units");

        TextBlock errorText = new TextBlock
        {
            Foreground = new SolidColorBrush(Color.FromRgb(0xE7, 0x4C, 0x3C)),
            FontSize = 11,
            Margin = new Thickness(2, 3, 0, 0),
            Visibility = Visibility.Collapsed
        };

        Grid.SetRow(label, 0);
        Grid.SetColumn(label, 0);
        Grid.SetRow(textBox, 0);
        Grid.SetColumn(textBox, 1);
        Grid.SetRow(errorText, 1);
        Grid.SetColumn(errorText, 1);

        floorGrid.Children.Add(label);
        floorGrid.Children.Add(textBox);
        floorGrid.Children.Add(errorText);

        return floorGrid;
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;

        if (!ValidateFields()) return;
        if (!ValidateFloors()) return;

        City selectedCity = (City)CityComboBox.SelectedItem;
        int numberOfFloors = int.Parse(FloorsTextBox.Text);

        long buildingId = _buildingService.CreateBuilding(
            StreetTextBox.Text,
            NumberTextBox.Text,
            SettlementTextBox.Text,
            selectedCity.Id,
            numberOfFloors,
            _currentUser.Id
        );

        CreateFloorsAndUnits(buildingId);
        SaveImages(buildingId);

        MessageBox.Show("A new building has been registered successfully!\nPress OK to continue.", "Success");
        Close();
    }

    private bool ValidateFields()
    {
        ClearFieldErrors();
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(StreetTextBox.Text))
        {
            ShowFieldError(StreetErrorText, "Street is required.");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(NumberTextBox.Text))
        {
            ShowFieldError(NumberErrorText, "Number is required.");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(SettlementTextBox.Text))
        {
            ShowFieldError(SettlementErrorText, "Settlement is required.");
            isValid = false;
        }

        if (CountryComboBox.SelectedItem == null)
        {
            ShowFieldError(CountryErrorText, "Please select a country.");
            isValid = false;
        }

        if (CityComboBox.SelectedItem == null)
        {
            ShowFieldError(CityErrorText, "Please select a city.");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(FloorsTextBox.Text))
        {
            ShowFieldError(FloorsErrorText, "Number of floors is required.");
            isValid = false;
        }

        if (!isValid)
            return false;

        City city = (City)CityComboBox.SelectedItem!;
        if (_buildingService.BuildingExists(StreetTextBox.Text, NumberTextBox.Text, city.Id))
        {
            ShowFieldError(StreetErrorText, "A building at this address already exists.");
            return false;
        }

        return true;
    }

    private bool ValidateFloors()
    {
        if (FloorsStackPanel.Children.Count == 0)
        {
            ShowFieldError(FloorsErrorText, "Please confirm the number of floors first.");
            return false;
        }

        bool isValid = true;

        foreach (Grid floorGrid in FloorsStackPanel.Children.OfType<Grid>())
        {
            TextBox unitTextBox = (TextBox)floorGrid.Children[1];
            TextBlock rowErrorText = (TextBlock)floorGrid.Children[2];

            if (string.IsNullOrWhiteSpace(unitTextBox.Text))
            {
                ShowFieldError(rowErrorText, "Please enter at least one unit number for this floor.");
                isValid = false;
            }
            else
            {
                HideFieldError(rowErrorText);
            }
        }

        return isValid;
    }

    private void ShowFieldError(TextBlock errorText, string message)
    {
        errorText.Text = message;
        errorText.Visibility = Visibility.Visible;
    }

    private void HideFieldError(TextBlock errorText)
    {
        errorText.Visibility = Visibility.Collapsed;
    }

    private void ClearFieldErrors()
    {
        HideFieldError(StreetErrorText);
        HideFieldError(NumberErrorText);
        HideFieldError(SettlementErrorText);
        HideFieldError(CountryErrorText);
        HideFieldError(CityErrorText);
        HideFieldError(FloorsErrorText);
    }

    private void CreateFloorsAndUnits(long buildingId)
    {
        foreach (Grid floorGrid in FloorsStackPanel.Children)
        {
            TextBox unitTextBox = (TextBox)floorGrid.Children[1];
            int floorNumber = (int)unitTextBox.Tag;
            long floorId = _buildingService.CreateFloor(buildingId, floorNumber);
            CreateUnitsForFloor(floorId, unitTextBox.Text);
        }
    }

    private void CreateUnitsForFloor(long floorId, string unitsText)
    {
        foreach (string unit in unitsText.Split(','))
        {
            string trimmed = unit.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                _buildingService.CreateUnit(floorId, trimmed);
        }
    }

    private void SaveImages(long buildingId)
    {
        string imagesFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "images", "buildings");
        Directory.CreateDirectory(imagesFolder);

        foreach (string imagePath in _selectedImagePaths)
        {
            string fileName = Path.GetFileName(imagePath);
            string destPath = Path.Combine(imagesFolder, fileName);
            File.Copy(imagePath, destPath, true);

            string relativePath = Path.Combine("images", "buildings", fileName);
            _buildingService.SaveBuildingImage(buildingId, relativePath);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        _activeFloating?.Close();
        _activeFloating = null;
        Close();
    }

    private void ChooseImages_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp;*.jfif"
        };

        if (dialog.ShowDialog() == true)
        {
            _selectedImagePaths.AddRange(dialog.FileNames);
            RefreshImagesPreview();
        }
    }

    private void RemoveImage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string imagePath)
        {
            _selectedImagePaths.Remove(imagePath);
            RefreshImagesPreview();
        }
    }

    private void RefreshImagesPreview()
    {
        ImagesPreview.ItemsSource = null;
        ImagesPreview.ItemsSource = _selectedImagePaths;
    }
}