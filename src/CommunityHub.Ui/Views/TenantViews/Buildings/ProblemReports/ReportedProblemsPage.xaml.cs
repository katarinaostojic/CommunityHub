using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.ProblemReports;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ProblemReports;
using CommunityHub.Ui.Views.TenantViews.Dialogs.Buildings;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews.Buildings;

public partial class ReportedProblemsPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly ReportedProblemsViewModel _viewModel;

    public ReportedProblemsPage(User user, BuildingMembershipDto membership)
    {
        InitializeComponent();
        _user = user;
        _membership = membership;

        ProblemReportService problemReportService = Injector.CreateInstance<ProblemReportService>();
        _viewModel = new ReportedProblemsViewModel(problemReportService, user.Id, membership.BuildingId);
        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        BuildingInfoTextBlock.Text = $"{_membership.BuildingFullAddress}, {_membership.BuildingNeighborhood}";
        AppMenu.Initialize(_user);
    }

    private void ReportProblemButton_Click(object sender, RoutedEventArgs e)
    {
        ReportProblemDialogViewModel dialogViewModel = new ReportProblemDialogViewModel(
            BuildingInfoTextBlock.Text);

        ReportProblemDialog dialog = new ReportProblemDialog(dialogViewModel);
        dialog.Owner = Window.GetWindow(this);

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.CreateReport(_user, dialog.Description, dialog.SelectedPriority);

        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            "✔ Problem report sent successfully.");
    }

    private void FilterAllButton_Click(object sender, RoutedEventArgs e) => _viewModel.FilterAll();

    private void FilterUnresolvedButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterUnresolved();

    private void FilterPotentiallySolvedButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterPotentiallySolved();

    private void FilterSolvedButton_Click(object sender, RoutedEventArgs e) =>
        _viewModel.FilterSolved();

    private void ConfirmResolvedButton_Click(object sender, RoutedEventArgs e)
    {
        ProblemReportRowViewModel report = (ProblemReportRowViewModel)((Button)sender).Tag;
        _viewModel.ConfirmResolved(report.Id);

        NotificationBanner.ShowSuccess(
            SuccessBanner,
            SuccessTextBlock,
            "✔ Problem confirmed as resolved. Thank you for your feedback.");
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) => AppMenu.Open();
}