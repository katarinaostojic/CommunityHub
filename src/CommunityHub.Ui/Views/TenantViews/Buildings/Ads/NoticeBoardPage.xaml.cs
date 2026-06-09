using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.Ads;
using CommunityHub.Application.Services.Reports;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.Helpers.Tenant.Demo.Building.Ads;
using CommunityHub.Ui.Helpers.Tenant.NoticeBoard;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NoticeBoardPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly NoticeBoardViewModel _viewModel;
    private readonly NoticeBoardFilterController _filterController;
    private readonly NoticeBoardActionsController _actionsController;
    private readonly NoticeBoardReportController _reportController;
    private readonly NoticeBoardDemoController _demoController;
    private readonly bool _startDemo;

    public NoticeBoardPage(User user, BuildingMembershipDto membership, bool startDemo = false)
    {
        InitializeComponent();

        _user = user;
        _membership = membership;
        _startDemo = startDemo;
        _viewModel = CreateViewModel();

        NoticeBoardNavigationHelper navigationHelper =
            new NoticeBoardNavigationHelper(_user, _membership, _viewModel, this);

        _filterController = new NoticeBoardFilterController(
            _viewModel,
            CategoryComboBox,
            FilterAllButton,
            FilterOfferingButton,
            FilterSeekingButton,
            this);

        _actionsController = new NoticeBoardActionsController(
            _viewModel,
            navigationHelper,
            this,
            SuccessBanner,
            SuccessTextBlock,
            RestoreAdButton);

        _reportController = new NoticeBoardReportController(
            _viewModel,
            this,
            ExportSuccessBanner,
            ExportSuccessTextBlock);

        _demoController = new NoticeBoardDemoController(
            DemoButton,
            _viewModel,
            _filterController,
            _user,
            _membership,
            this,
            CategoryComboBox,
            AdsScrollViewer,
            RestoreAdButton);

        DataContext = _viewModel;
        InitializePageData();

        if (_startDemo)
            Loaded += StartDemoOnLoaded;
    }

    private async void StartDemoOnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= StartDemoOnLoaded;
        await _demoController.ToggleAsync();
    }

    public void ShowBookingSuccess()
    {
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Slots booked successfully!");
    }

    private NoticeBoardViewModel CreateViewModel()
    {
        AdService adService = Injector.CreateInstance<AdService>();
        AdNotificationService notificationService = Injector.CreateInstance<AdNotificationService>();
        AdsReportService reportService = Injector.CreateInstance<AdsReportService>();
        AdsPdfExporter pdfExporter = Injector.CreateInstance<AdsPdfExporter>();

        return new NoticeBoardViewModel(
            adService,
            notificationService,
            reportService,
            pdfExporter,
            _membership,
            _user.Id,
            _user.DisplayName);
    }

    private void InitializePageData()
    {
        UserNameTextBlock.Text = _user.DisplayName;
        NotificationBell.Initialize(_user);
        AppMenu.Initialize(_user);

        CategoryComboBox.ItemsSource = _viewModel.CategoryOptions;
        CategoryComboBox.SelectedIndex = 0;
        _filterController.ApplyTypeFilter("All");
    }

    private void FilterTypeButton_Click(object sender, RoutedEventArgs e)
    {
        _filterController.ApplyTypeFilter((string)((Button)sender).Tag);
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _filterController.ApplySelectedCategory();
    }

    private void NewAdButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationService.Navigate(new NewAdPage(_user, _membership));
    }

    private async void DemoButton_Click(object sender, RoutedEventArgs e)
    {
        await _demoController.ToggleAsync();
    }

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        _actionsController.Archive((AdViewModel)((Button)sender).Tag);
    }

    private void RestoreAdButton_Click(object sender, RoutedEventArgs e)
    {
        _actionsController.RestoreLastArchivedAd();
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        _actionsController.OpenSlots((AdViewModel)((Button)sender).Tag);
    }

    private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
    {
        _actionsController.OpenDetails((AdViewModel)((Button)sender).Tag);
    }

    private void DismissNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        _actionsController.DismissNotification((long)((Button)sender).Tag);
    }

    private void ViewSlotsFromNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        _actionsController.OpenSlotsFromNotification((AdNotificationViewModel)((Button)sender).DataContext);
    }

    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        _reportController.ExportPdf();
    }

    private void DismissExportSuccessButton_Click(object sender, RoutedEventArgs e)
    {
        _reportController.DismissExportSuccess();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        AppMenu.Open();
    }
}