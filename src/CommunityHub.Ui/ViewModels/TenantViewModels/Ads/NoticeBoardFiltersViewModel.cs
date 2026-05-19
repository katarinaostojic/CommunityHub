using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Ads;

public class NoticeBoardFiltersViewModel : BaseViewModel
{
    private readonly AdService _adService;
    private readonly long _buildingId;

    private string _allFilterText = "All (0)";
    private string _offeringFilterText = "Offering (0)";
    private string _seekingFilterText = "Seeking (0)";

    public NoticeBoardFiltersViewModel(AdService adService, long buildingId)
    {
        _adService = adService;
        _buildingId = buildingId;
        CategoryOptions = BuildCategoryOptions();
    }

    public AdType? TypeFilter { get; private set; }
    public AdCategory? CategoryFilter { get; private set; }
    public List<string> CategoryOptions { get; }

    public string AllFilterText
    {
        get => _allFilterText;
        private set => SetProperty(ref _allFilterText, value);
    }

    public string OfferingFilterText
    {
        get => _offeringFilterText;
        private set => SetProperty(ref _offeringFilterText, value);
    }

    public string SeekingFilterText
    {
        get => _seekingFilterText;
        private set => SetProperty(ref _seekingFilterText, value);
    }

    public void SelectAll()
    {
        TypeFilter = null;
    }

    public void SelectOffering()
    {
        TypeFilter = AdType.Offering;
    }

    public void SelectSeeking()
    {
        TypeFilter = AdType.Seeking;
    }

    public void SelectCategory(int selectedIndex)
    {
        CategoryFilter = selectedIndex == 0
            ? null
            : (AdCategory)(selectedIndex - 1);
    }

    public void LoadCounts()
    {
        AllFilterText = BuildFilterText("All", null);
        OfferingFilterText = BuildFilterText("Offering", AdType.Offering);
        SeekingFilterText = BuildFilterText("Seeking", AdType.Seeking);
    }

    private string BuildFilterText(string label, AdType? type)
    {
        int count = _adService.CountFilteredActiveByBuilding(
            _buildingId,
            type,
            CategoryFilter);

        return $"{label} ({count})";
    }

    private static List<string> BuildCategoryOptions()
    {
        List<string> options = new() { "All Categories" };

        foreach (AdCategory category in Enum.GetValues<AdCategory>())
            options.Add(category.ToDisplayString());

        return options;
    }
}