using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.Mappings.Buildings;
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
        var buildings = _buildingService.GetAllByManager(_currentUser.Id)
                                        .Select(b => b.ToDto())
                                        .ToList();
        BuildingsItemsControl.ItemsSource = buildings;
    }

    private void ViewNoticeboard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is BuildingDto building)
        {
            NavigationService.Navigate(new ManagerNoticeBoardDetailsPage(_currentUser, building));
        }
    }
}