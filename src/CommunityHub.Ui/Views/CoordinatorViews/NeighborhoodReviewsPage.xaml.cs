using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class NeighborhoodReviewsPage : Page
{
    public NeighborhoodReviewsPage(long coordinatorId)
    {
        InitializeComponent();
        var reviewService = Injector.CreateInstance<CoordinatorReviewService>();
        var neighborhoodService = Injector.CreateInstance<NeighborhoodService>();
        DataContext = new NeighborhoodReviewsViewModel(reviewService, neighborhoodService, coordinatorId);
    }
}