using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.Ads;
using CommunityHub.Application.Services.Reports;
using CommunityHub.Ui.Controls;
using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.NoticeBoard;

public static class NoticeBoardPageInitializer
{
    public static NoticeBoardViewModel CreateViewModel(User user, BuildingMembershipDto membership)
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
            membership,
            user.Id,
            user.DisplayName);
    }

    public static void InitializePageData(
        User user,
        NoticeBoardViewModel viewModel,
        TextBlock userNameTextBlock,
        NotificationBell notificationBell,
        MenuPanel appMenu,
        ComboBox categoryComboBox,
        NoticeBoardFilterController filterController)
    {
        userNameTextBlock.Text = user.DisplayName;
        notificationBell.Initialize(user);
        appMenu.Initialize(user);

        categoryComboBox.ItemsSource = viewModel.CategoryOptions;
        categoryComboBox.SelectedIndex = 0;
        filterController.ApplyTypeFilter("All");
    }
}