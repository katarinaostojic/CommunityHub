using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class BuildingDetailsPage : Page
{
    private readonly User _currentUser;
    private readonly Building _building;
    private readonly BuildingService _buildingService;
    private readonly CommonRoomService _commonRoomService;

    public BuildingDetailsPage(User user, Building building)
    {
        InitializeComponent();
        _currentUser = user;
        _building = building;
        _buildingService = ServiceFactory.CreateBuildingService();
        _commonRoomService = ServiceFactory.CreateCommonRoomService();
        LoadBuildingInfo();
    }

    private void LoadBuildingInfo()
    {
        BuildingTitleText.Text = $"{_building.Street} {_building.StreetNumber}";
        TxtStreet.Text = $"Street: {_building.Street} {_building.StreetNumber}";
        TxtNeighborhood.Text = $"Neighborhood: {_building.Neighborhood}";
        TxtCity.Text = $"City: {_building.City.Name}, {_building.City.Country.Name}";
        TxtFloors.Text = $"Floors: {_building.NumberOfFloors}";
        TxtUnits.Text = $"Total units: {_building.TotalUnits}";
        MembershipsDataGrid.ItemsSource = _building.Memberships;
        ImagesItemsControl.ItemsSource = _building.Images;
    }

    private void LoadCommonRooms()
    {
        List<CommonRoom> rooms = _commonRoomService.GetByBuilding(_building.Id);
        CommonRoomsItemsControl.ItemsSource = rooms;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.GoBack();
    }

    private void TabBuildingInfo_Click(object sender, RoutedEventArgs e)
    {
        PanelBuildingInfo.Visibility = Visibility.Visible;
        PanelCommonRoom.Visibility = Visibility.Collapsed;
        SetActiveTab(TabBuildingInfo);
        SetInactiveTab(TabCommonRoom);
    }

    private void TabCommonRoom_Click(object sender, RoutedEventArgs e)
    {
        PanelBuildingInfo.Visibility = Visibility.Collapsed;
        PanelCommonRoom.Visibility = Visibility.Visible;
        SetActiveTab(TabCommonRoom);
        SetInactiveTab(TabBuildingInfo);
        LoadCommonRooms();
    }

    private void SetActiveTab(Button tab)
    {
        tab.Foreground = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        tab.BorderThickness = new Thickness(0, 0, 0, 3);
        tab.BorderBrush = new SolidColorBrush(Color.FromRgb(41, 128, 185));
        tab.FontWeight = FontWeights.SemiBold;
    }

    private void SetInactiveTab(Button tab)
    {
        tab.Foreground = new SolidColorBrush(Color.FromRgb(127, 140, 141));
        tab.BorderThickness = new Thickness(0);
        tab.FontWeight = FontWeights.Normal;
    }

    private void AddCommonRoom_Click(object sender, RoutedEventArgs e)
    {
        AddCommonRoomDialog dialog = new AddCommonRoomDialog(_currentUser, _building, _commonRoomService);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
        LoadCommonRooms();
    }

    private void ViewRequests_Click(object sender, RoutedEventArgs e)
    {
        // Dolazi kasnije
    }
}