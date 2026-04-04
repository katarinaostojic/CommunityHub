using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Services.TenantServices;
using CommunityHub.Ui.Views.TenantViews;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CommunityHub.Ui.Helpers;
using System.IO;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class BuildingDetailsPage : Page
{
    private readonly Building _building;
    private readonly User _user;
    private readonly BuildingService _buildingService;
    private readonly BuildingAccessRequestService _requestService;
    private int _currentImageIndex = 0;

    public BuildingDetailsPage(Building building, User user)
    {
        InitializeComponent();
        _buildingService = new BuildingService();
        _requestService = new BuildingAccessRequestService();
        _building = _buildingService.GetById(building.Id) ?? building;
        _user = user;
        LoadBuildingDetails();
    }

    private void LoadBuildingDetails()
    {
        DisplayAddress();
        DisplayInfoCards();
        DisplayCurrentImage();
        UserNameTextBlock.Text = char.ToUpper(_user.Name[0]) + _user.Name.Substring(1).ToLower();
        AppMenu.Initialize(_user);
    }

    private void DisplayAddress()
    {
        AddressText.Text = $"{_building.Street} {_building.StreetNumber}";
        NeighborhoodText.Text = _building.Neighborhood;
        CityCountryText.Text = $"{_building.City.Name}, {_building.City.Country.Name}";
    }

    private void DisplayInfoCards()
    {
        FloorsText.Text = _building.NumberOfFloors.ToString();
        TotalUnitsText.Text = _building.TotalUnits.ToString();
        VacanciesText.Text = _buildingService.GetVacancies(_building).ToString();
        PendingRequestsText.Text = _requestService.GetPendingRequestsCount(_building.Id).ToString();
    }

    private void DisplayCurrentImage()
    {
        if (_building.Images.Count == 0)
        {
            BuildingImage.Source = null;
            ImageCounterText.Text = string.Empty;
            SwipeText.Visibility = Visibility.Collapsed;
            return;
        }

        string fullPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            _building.Images[_currentImageIndex].Path.Replace('/', Path.DirectorySeparatorChar)
        );

        try { BuildingImage.Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)); }
        catch { BuildingImage.Source = null; }

        ImageCounterText.Text = $"{_currentImageIndex + 1}/{_building.Images.Count}";
        UpdateImageDots();
    }

    private void UpdateImageDots()
    {
        ImageDots.ItemsSource = _building.Images.Select((_, index) =>
            index == _currentImageIndex ? "White" : "#88FFFFFF").ToList();
    }

    private void PrevImageButton_Click(object sender, RoutedEventArgs e)
    {
        if (_building.Images.Count == 0) return;
        _currentImageIndex = (_currentImageIndex - 1 + _building.Images.Count) % _building.Images.Count;
        DisplayCurrentImage();
    }

    private void NextImageButton_Click(object sender, RoutedEventArgs e)
    {
        if (_building.Images.Count == 0) return;
        _currentImageIndex = (_currentImageIndex + 1) % _building.Images.Count;
        DisplayCurrentImage();
    }

    private void RequestAccessButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ShowRequestAccessDialog(_building)) return;

        DisplayInfoCards();
        ViewRequestsButton.Visibility = Visibility.Visible;
        TenantBanner.ShowSuccess(SuccessBanner, SuccessTextBlock,
            $"✔ Request Sent Successfully!");
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new MyRequestsPage(_user));
    }

    private bool ShowRequestAccessDialog(Building building)
    {
        RequestAccessDialog dialog = new RequestAccessDialog(building, _user);
        dialog.Owner = Window.GetWindow(this);
        return dialog.ShowDialog() == true;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }
}