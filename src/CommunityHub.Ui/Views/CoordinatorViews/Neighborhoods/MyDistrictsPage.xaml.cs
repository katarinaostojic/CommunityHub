using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class MyDistrictsPage : Page
{
    private readonly MyDistrictsViewModel _viewModel;

    public MyDistrictsPage(long userId)
    {
        InitializeComponent();
        NeighborhoodService neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        NeighborhoodAccessRequestService requestService = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        _viewModel = new MyDistrictsViewModel(neighborhoodService, requestService, userId);
        DataContext = _viewModel;
    }

    private void DistrictCard_Click(object sender, RoutedEventArgs e)
    {
        var card = (DistrictCardViewModel)((Button)sender).Tag;
        CoordinatorMainWindow.Instance.NavigateTo(new NeighborhoodDetailsPage(card.District), card.District.Name);
    }

    private void AddDistrictButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new RegisterNeighborhoodPage(_viewModel.CoordinatorId), "Add New District");
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateToDashboard();
    }
}