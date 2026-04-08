using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Building;
using CommunityHub.Application.Services;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class MyBuildingsPage : Page
{
    private readonly User _currentUser;
    private readonly BuildingService _buildingService;

    public MyBuildingsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _buildingService = new BuildingService();
        LoadBuildings();
    }

    private void LoadBuildings()
    {
        List<Building> buildings = _buildingService.GetAllByManager(_currentUser.Id);
        BuildingsItemsControl.ItemsSource = buildings;
    }

    private void RegisterBuilding_Click(object sender, RoutedEventArgs e)
    {
        RegisterBuildingDialog dialog = new RegisterBuildingDialog(_currentUser);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
        LoadBuildings();
    }

    private void BuildingCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is Building building)
        {
            // Building details - dolazi kasnije
        }
    }
}