using CommunityHub.Ui.ViewModels.TenantViewModels.Ads.NoticeBoard;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Helpers.Tenant.NoticeBoard;

public class NoticeBoardFilterController
{
    private readonly NoticeBoardViewModel _viewModel;
    private readonly ComboBox _categoryComboBox;
    private readonly Button _filterAllButton;
    private readonly Button _filterOfferingButton;
    private readonly Button _filterSeekingButton;
    private readonly FrameworkElement _resourceOwner;

    public NoticeBoardFilterController(
        NoticeBoardViewModel viewModel,
        ComboBox categoryComboBox,
        Button filterAllButton,
        Button filterOfferingButton,
        Button filterSeekingButton,
        FrameworkElement resourceOwner)
    {
        _viewModel = viewModel;
        _categoryComboBox = categoryComboBox;
        _filterAllButton = filterAllButton;
        _filterOfferingButton = filterOfferingButton;
        _filterSeekingButton = filterSeekingButton;
        _resourceOwner = resourceOwner;
    }

    public void ApplyTypeFilter(string filterType)
    {
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

        SetTypeFilterButtonStyles(filterType);
    }

    public void ApplySelectedCategory()
    {
        _viewModel.FilterByCategory(_categoryComboBox.SelectedIndex);
    }

    public void SelectCategoryIfAvailable(int index)
    {
        if (_categoryComboBox.Items.Count <= index)
            return;

        _categoryComboBox.SelectedIndex = index;
        _viewModel.FilterByCategory(index);
    }

    private void SetTypeFilterButtonStyles(string activeFilter)
    {
        Style inactiveStyle = (Style)_resourceOwner.FindResource("FilterChipButton");
        Style activeStyle = (Style)_resourceOwner.FindResource("FilterChipButtonActive");

        _filterAllButton.Style = activeFilter == "All" ? activeStyle : inactiveStyle;
        _filterOfferingButton.Style = activeFilter == "Offering" ? activeStyle : inactiveStyle;
        _filterSeekingButton.Style = activeFilter == "Seeking" ? activeStyle : inactiveStyle;
    }
}