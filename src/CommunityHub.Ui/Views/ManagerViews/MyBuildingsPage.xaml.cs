using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.Mappings.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class MyBuildingsPage : Page
{
    private readonly User _currentUser;
    private readonly BuildingService _buildingService;
    private List<BuildingDto> _allBuildings = new();
    private int _currentPage = 1;
    private const int PageSize = 6;

    public MyBuildingsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _buildingService = Injector.CreateInstance<BuildingService>();
        LoadBuildings();
    }

    private void LoadBuildings()
    {
        _allBuildings = _buildingService.GetAllByManager(_currentUser.Id)
                                        .Select(b => b.ToDto())
                                        .ToList();
        _currentPage = 1;
        ShowCurrentPage();
    }

    private void ShowCurrentPage()
    {
        int totalPages = (int)Math.Ceiling(_allBuildings.Count / (double)PageSize);
        if (totalPages == 0) totalPages = 1;

        var pageItems = _allBuildings
            .Skip((_currentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();

        BuildingsItemsControl.ItemsSource = pageItems;
        PageIndicator.Text = $"Page {_currentPage} / {totalPages}";
        PrevButton.IsEnabled = _currentPage > 1;
        NextButton.IsEnabled = _currentPage < totalPages;
    }

    private void PrevButton_Click(object sender, RoutedEventArgs e)
    {
        _currentPage--;
        ShowCurrentPage();
    }

    private void NextButton_Click(object sender, RoutedEventArgs e)
    {
        _currentPage++;
        ShowCurrentPage();
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
    //    if (sender is Border border && border.DataContext is BuildingDto building)
    //   {
    //        BuildingDto? fullBuilding = _buildingService.GetById(building.Id);
    //       if (fullBuilding == null) return;
     //       NavigationService.Navigate(new BuildingDetailsPage(_currentUser, fullBuilding));
      //  }
    }
}