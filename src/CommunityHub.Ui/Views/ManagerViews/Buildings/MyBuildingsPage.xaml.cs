using CommunityHub.Application.Domain;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class MyBuildingsPage : Page
{
    private readonly User _currentUser;
    private readonly MyBuildingsViewModel _viewModel;

    public MyBuildingsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
        _viewModel = new MyBuildingsViewModel(_currentUser.Id);
        DataContext = _viewModel;
    }

    private void RegisterBuilding_Click(object sender, RoutedEventArgs e)
    {
        RegisterBuildingDialog dialog = new RegisterBuildingDialog(_currentUser);
        dialog.Owner = Window.GetWindow(this);
        dialog.ShowDialog();
        _viewModel.LoadBuildings();
    }

    private void BuildingCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is BuildingDto building)
        {
            NavigationService.Navigate(new BuildingDetailsPage(_currentUser, building.Id));
        }
    }

    private void PrevButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.PreviousPage();

    private void NextButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.NextPage();
}