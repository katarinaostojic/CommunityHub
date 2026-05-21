using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.DTOs.Ads;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NoticeBoard;
using CommunityHub.Ui.Views.TenantViews;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers;

public class NoticeBoardNavigationHelper
{
    private readonly User _user;
    private readonly BuildingMembershipDto _membership;
    private readonly NoticeBoardViewModel _viewModel;
    private readonly NoticeBoardPage _noticeBoardPage;

    public NoticeBoardNavigationHelper(
        User user,
        BuildingMembershipDto membership,
        NoticeBoardViewModel viewModel,
        NoticeBoardPage noticeBoardPage)
    {
        _user = user;
        _membership = membership;
        _viewModel = viewModel;
        _noticeBoardPage = noticeBoardPage;
    }

    public Page? CreateBookSlotsPage(AdViewModel adViewModel)
    {
        AdDto? theirAd = _viewModel.GetAdById(adViewModel.Id);
        if (theirAd == null || adViewModel.MyMatchingAdId == null) return null;

        AdDto? myAd = _viewModel.GetAdById(adViewModel.MyMatchingAdId.Value);
        return myAd == null
            ? null
            : new BookSlotsPage(_user, _membership, theirAd, myAd, _noticeBoardPage);
    }

    public Page? CreateAdDetailsPage(AdViewModel adViewModel) =>
        CreateAdDetailsPage(adViewModel.Id);

    public Page? CreateAdDetailsPage(AdNotificationViewModel notification) =>
        CreateAdDetailsPage(notification.AdId);

    private Page? CreateAdDetailsPage(long adId)
    {
        AdDto? ad = _viewModel.GetAdById(adId);
        return ad == null ? null : new AdDetailsPage(_user, _membership, ad);
    }
}