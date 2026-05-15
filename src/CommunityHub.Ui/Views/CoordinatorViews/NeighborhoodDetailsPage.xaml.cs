using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class NeighborhoodDetailsPage : Page
{
    private readonly NeighborhoodDto _neighborhood;
    private readonly NeighborhoodDetailsViewModel _viewModel;

    public NeighborhoodDetailsPage(NeighborhoodDto neighborhood)
    {
        InitializeComponent();
        _neighborhood = neighborhood;
        NeighborhoodMembershipService membershipService = Injector.CreateInstance<NeighborhoodMembershipService>();
        _viewModel = new NeighborhoodDetailsViewModel(membershipService, neighborhood.Id);
        DataContext = _viewModel;
        NeighborhoodNameText.Text = neighborhood.Name;
    }

    private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_neighborhood.CoordinatorId), "My Districts");
    }

    private void MeetingsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingsPage(_neighborhood.CoordinatorId, _neighborhood.Id), "Meetings");
    }
}