using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Ads;
using CommunityHub.Application.Services.Reports;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NoticeBoard;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.TenantViews;

public partial class NoticeBoardPage : Page
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly NoticeBoardViewModel _viewModel;
    private readonly NoticeBoardNavigationHelper _navigationHelper;
    private long? _lastArchivedAdId;

    public NoticeBoardPage(User user, BuildingMembershipDto membership)
    {
        InitializeComponent();

        _user = user;
        _membership = membership;

        AdService adService = Injector.CreateInstance<AdService>();
        AdNotificationService notificationService = Injector.CreateInstance<AdNotificationService>();
        AdsReportService reportService = Injector.CreateInstance<AdsReportService>();
        AdsPdfExporter pdfExporter = Injector.CreateInstance<AdsPdfExporter>();

		_viewModel = new NoticeBoardViewModel(
			adService,
			notificationService,
			reportService,
			pdfExporter,
			_membership,
			_user.Id,
			_user.DisplayName);

		_navigationHelper = new NoticeBoardNavigationHelper(_user, _membership, _viewModel, this);

        DataContext = _viewModel;

        UserNameTextBlock.Text = _user.DisplayName;
        NotificationBell.Initialize(_user);
        AppMenu.Initialize(_user);

        CategoryComboBox.ItemsSource = _viewModel.CategoryOptions;
        CategoryComboBox.SelectedIndex = 0;
    }

    public void ShowBookingSuccess() =>
        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Slots booked successfully!");

    private void FilterTypeButton_Click(object sender, RoutedEventArgs e)
    {
        string filterType = (string)((Button)sender).Tag;

        switch (filterType)
        {
            case "All":
                _viewModel.FilterAll();
                break;
            case "Offering":
                _viewModel.FilterOffering();
                break;
            case "Seeking":
                _viewModel.FilterSeeking();
                break;
        }
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        _viewModel.FilterByCategory(CategoryComboBox.SelectedIndex);

    private void NewAdButton_Click(object sender, RoutedEventArgs e) =>
        NavigationService.Navigate(new NewAdPage(_user, _membership));

    private void ArchiveButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = (AdViewModel)((Button)sender).Tag;

        _lastArchivedAdId = ad.Id;
        _viewModel.ArchiveAd(ad.Id);

        NotificationBanner.ShowSuccess(SuccessBanner, SuccessTextBlock, "✔ Ad archived successfully.");
        RestoreAdButton.Visibility = Visibility.Visible;
    }

    private void RestoreAdButton_Click(object sender, RoutedEventArgs e)
    {
        if (_lastArchivedAdId == null)
            return;

        _viewModel.RestoreAd(_lastArchivedAdId.Value);
        _lastArchivedAdId = null;

        RestoreAdButton.Visibility = Visibility.Collapsed;
        SuccessBanner.Visibility = Visibility.Collapsed;
    }

    private void ViewSlotsButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = (AdViewModel)((Button)sender).Tag;
        Page? page = _navigationHelper.CreateBookSlotsPage(ad);

        if (page != null)
            NavigationService.Navigate(page);
    }

    private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
    {
        AdViewModel ad = (AdViewModel)((Button)sender).Tag;
        Page? page = _navigationHelper.CreateAdDetailsPage(ad);

        if (page != null)
            NavigationService.Navigate(page);
    }

    private void DismissNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        long notificationId = (long)((Button)sender).Tag;
        _viewModel.Notifications.Dismiss(notificationId);
    }

    private void ViewSlotsFromNotificationButton_Click(object sender, RoutedEventArgs e)
    {
        AdNotificationViewModel notification = (AdNotificationViewModel)((Button)sender).DataContext;
        Page? page = _navigationHelper.CreatePageFromNotification(notification);

        if (page != null)
            NavigationService.Navigate(page);
    }

    private void ExportPdfButton_Click(object sender, RoutedEventArgs e)
    {
        ExportReportDialog dialog = new ExportReportDialog
        {
            Owner = Window.GetWindow(this)
        };

        if (dialog.ShowDialog() != true)
            return;

        string? filePath = SelectPdfFilePath(dialog.DateFrom, dialog.DateTo);

        if (filePath == null)
            return;

        _viewModel.ExportReport(filePath, dialog.DateFrom, dialog.DateTo);

        NotificationBanner.ShowSuccess(
            ExportSuccessBanner,
            ExportSuccessTextBlock,
            "✔ PDF exported successfully.");

        PdfViewer.Open(filePath);
    }

    private static string? SelectPdfFilePath(DateOnly dateFrom, DateOnly dateTo)
    {
        SaveFileDialog dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"tenant-ads-report-{dateFrom:yyyy-MM-dd}-{dateTo:yyyy-MM-dd}.pdf"
        };

        return dialog.ShowDialog() == true
            ? dialog.FileName
            : null;
    }

    private void DismissExportSuccessButton_Click(object sender, RoutedEventArgs e)
    {
        ExportSuccessBanner.Visibility = Visibility.Collapsed;
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e) =>
        AppMenu.Open();
}