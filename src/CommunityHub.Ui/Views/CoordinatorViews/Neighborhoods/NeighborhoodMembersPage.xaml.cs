using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class NeighborhoodMembersPage : Page
{
    private readonly NeighborhoodDto _neighborhood;

    public NeighborhoodMembersPage(NeighborhoodDto neighborhood)
    {
        InitializeComponent();
        _neighborhood = neighborhood;
        NeighborhoodMembershipService membershipService = Injector.CreateInstance<NeighborhoodMembershipService>();
        NeighborhoodDetailsViewModel viewModel = new NeighborhoodDetailsViewModel(membershipService, neighborhood.Id);
        DataContext = viewModel;
        NeighborhoodNameText.Text = neighborhood.Name;
    }
}