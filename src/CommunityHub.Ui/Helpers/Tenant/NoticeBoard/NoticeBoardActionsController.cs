using CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.Ads.NoticeBoard;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.NoticeBoard;

public class NoticeBoardActionsController
{
    private readonly NoticeBoardViewModel _viewModel;
    private readonly NoticeBoardNavigationHelper _navigationHelper;
    private readonly Page _page;
    private readonly Border _successBanner;
    private readonly TextBlock _successTextBlock;
    private readonly Button _restoreAdButton;

    private long? _lastArchivedAdId;

    public NoticeBoardActionsController(
        NoticeBoardViewModel viewModel,
        NoticeBoardNavigationHelper navigationHelper,
        Page page,
        Border successBanner,
        TextBlock successTextBlock,
        Button restoreAdButton)
    {
        _viewModel = viewModel;
        _navigationHelper = navigationHelper;
        _page = page;
        _successBanner = successBanner;
        _successTextBlock = successTextBlock;
        _restoreAdButton = restoreAdButton;
    }

    public void Archive(AdViewModel ad)
    {
        _lastArchivedAdId = ad.Id;
        _viewModel.ArchiveAd(ad.Id);

        NotificationBanner.ShowSuccess(_successBanner, _successTextBlock, "✔ Ad archived successfully.");
        _restoreAdButton.Visibility = Visibility.Visible;
    }

    public void RestoreLastArchivedAd()
    {
        if (_lastArchivedAdId == null)
            return;

        _viewModel.RestoreAd(_lastArchivedAdId.Value);
        _lastArchivedAdId = null;

        _restoreAdButton.Visibility = Visibility.Collapsed;
        _successBanner.Visibility = Visibility.Collapsed;
    }

    public void OpenSlots(AdViewModel ad)
    {
        Page? page = _navigationHelper.CreateBookSlotsPage(ad);

        if (page != null)
            _page.NavigationService?.Navigate(page);
    }

    public void OpenDetails(AdViewModel ad)
    {
        Page? page = _navigationHelper.CreateAdDetailsPage(ad);

        if (page != null)
            _page.NavigationService?.Navigate(page);
    }

    public void DismissNotification(long notificationId)
    {
        _viewModel.Notifications.Dismiss(notificationId);
    }

    public void OpenSlotsFromNotification(AdNotificationViewModel notification)
    {
        Page? page = _navigationHelper.CreatePageFromNotification(notification);

        if (page != null)
            _page.NavigationService?.Navigate(page);
    }
}