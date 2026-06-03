using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class CategoryDetailsPage : Page
{
    public CategoryDetailsPage(CategoryBudgetItemViewModel category, long neighborhoodId)
    {
        InitializeComponent();
        var donationService = Injector.CreateInstance<DonationService>();
        DataContext = new CategoryDetailsViewModel(donationService, category, neighborhoodId);
    }
}