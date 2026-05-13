using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class ManagerNoticeBoardPage : Page
{
    private readonly User _currentUser;
    private readonly BuildingService _buildingService;

    public ManagerNoticeBoardPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _buildingService = Injector.CreateInstance<BuildingService>();
        LoadBuildings();
    }

    private void LoadBuildings()
    {
        List<Building> buildings = _buildingService.GetAllByManager(_currentUser.Id);
        BuildingsItemsControl.ItemsSource = buildings;
    }

    private void ViewNoticeboard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Building building)
        {
            Building? fullBuilding = _buildingService.GetById(building.Id);
            if (fullBuilding == null) return;
            NavigationService.Navigate(new ManagerNoticeBoardDetailsPage(_currentUser, fullBuilding));
        }
    }
}