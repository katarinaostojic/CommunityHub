using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Converters;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.Views.TenantViews;
using System.Windows;
using System.Windows.Controls;

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
        UserNameTextBlock.Text = _user.DisplayName;
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

        BuildingImage.Source = ImagePathConverter.LoadImage(_building.Images[_currentImageIndex].Path);

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
        if (!ShowBuildingRequestAccessDialog(_building)) return;

        DisplayInfoCards();
        ViewRequestsButton.Visibility = Visibility.Visible;
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock,
            $"✔ Request Sent Successfully!");
    }

    private void ViewRequestsButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new MyBuildingRequestsPage(_user));
    }

    private bool ShowBuildingRequestAccessDialog(Building building)
    {
        BuildingAccessRequestDialog dialog = new BuildingAccessRequestDialog(building, _user);
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