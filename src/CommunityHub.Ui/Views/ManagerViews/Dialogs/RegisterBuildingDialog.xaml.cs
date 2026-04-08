using CommunityHub.Application.Domain;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Services.ManagerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace CommunityHub.Ui.Views.ManagerViews.Dialogs;

public partial class RegisterBuildingDialog : Window
{
    private readonly User _currentUser;
    private readonly ManagerBuildingService _buildingService;
    private readonly CityDbRepository _cityRepository;
    private readonly CountryDbRepository _countryRepository;
    private List<string> _selectedImagePaths = new List<string>();

    public RegisterBuildingDialog(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _buildingService = new ManagerBuildingService();
        _cityRepository = new CityDbRepository();
        _countryRepository = new CountryDbRepository();
        LoadCountries();
        LoadCities();
    }

    private void LoadCountries()
    {
        var countries = _countryRepository.GetAll();
        CountryComboBox.ItemsSource = countries;
        CountryComboBox.DisplayMemberPath = "Name";
    }

    private void LoadCities()
    {
        var cities = _cityRepository.GetAll();
        CityComboBox.ItemsSource = cities;
        CityComboBox.DisplayMemberPath = "Name";
    }

    private void ConfirmFloors_Click(object sender, RoutedEventArgs e)
    {
        FloorsStackPanel.Children.Clear();

        if (!int.TryParse(FloorsTextBox.Text, out int numberOfFloors) || numberOfFloors <= 0)
        {
            MessageBox.Show("Please enter a valid number of floors.", "Error");
            return;
        }

        for (int i = 1; i <= numberOfFloors; i++)
        {
            Grid floorGrid = new Grid();
            floorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            floorGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            floorGrid.Margin = new Thickness(0, 10, 0, 10);

            TextBlock label = new TextBlock
            {
                Text = $"* Floor {i} units (e.g. 1,2,3)",
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14
            };

            TextBox textBox = new TextBox
            {
                Height = 35,
                FontSize = 14,
                Tag = i
            };
            textBox.GotFocus += TextBox_GotFocus;

            Grid.SetColumn(label, 0);
            Grid.SetColumn(textBox, 1);
            floorGrid.Children.Add(label);
            floorGrid.Children.Add(textBox);
            FloorsStackPanel.Children.Add(floorGrid);
        }
    }

    private void Register_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(StreetTextBox.Text) ||
            string.IsNullOrWhiteSpace(NumberTextBox.Text) ||
            string.IsNullOrWhiteSpace(SettlementTextBox.Text) ||
            CityComboBox.SelectedItem == null ||
            CountryComboBox.SelectedItem == null ||
            string.IsNullOrWhiteSpace(FloorsTextBox.Text))
        {
            MessageBox.Show("Please fill in all required fields.", "Error");
            return;
        }

        if (FloorsStackPanel.Children.Count == 0)
        {
            MessageBox.Show("Please confirm the number of floors first.", "Error");
            return;
        }

        City selectedCity = (City)CityComboBox.SelectedItem;
        int numberOfFloors = int.Parse(FloorsTextBox.Text);

        long buildingId = _buildingService.CreateBuilding(
            StreetTextBox.Text,
            NumberTextBox.Text,
            SettlementTextBox.Text,
            selectedCity.Id,
            numberOfFloors
        );

        foreach (Grid floorGrid in FloorsStackPanel.Children)
        {
            TextBox unitTextBox = (TextBox)floorGrid.Children[1];
            int floorNumber = (int)unitTextBox.Tag;

            long floorId = _buildingService.CreateFloorReturningId(buildingId, floorNumber);

            string[] units = unitTextBox.Text.Split(',');
            foreach (string unit in units)
            {
                string trimmed = unit.Trim();
                if (!string.IsNullOrEmpty(trimmed))
                    _buildingService.CreateUnit(floorId, trimmed);
            }
        }

        MessageBox.Show("A new building has been registered successfully!\nPress OK to continue.", "Success");
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ChooseImages_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Image files|*.jpg;*.jpeg;*.png;*.bmp"
        };

        if (dialog.ShowDialog() == true)
        {
            _selectedImagePaths.AddRange(dialog.FileNames);
            ImagesPreview.ItemsSource = null;
            ImagesPreview.ItemsSource = _selectedImagePaths;
        }
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        // Virtuelna tastatura dolazi kasnije
    }
}