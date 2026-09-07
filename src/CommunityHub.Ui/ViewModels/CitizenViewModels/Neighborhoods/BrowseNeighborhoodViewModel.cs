using CommunityHub.Ui.Helpers.Citizen;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class BrowseNeighborhoodViewModel : BaseViewModel
{
    private readonly NeighborhoodService _neighborhoodService;

    private List<NeighborhoodDto> _allNeighborhoods = new();
    private ObservableCollection<NeighborhoodDto> _filteredNeighborhoods = new();
    private string _resultsTitleText = string.Empty;

    public BrowseNeighborhoodViewModel(NeighborhoodService neighborhoodService)
    {
        _neighborhoodService = neighborhoodService;
        LoadNeighborhoods();
        LanguageManager.LanguageChanged += UpdateResultsTitle;
    }

    public ObservableCollection<NeighborhoodDto> FilteredNeighborhoods
    {
        get => _filteredNeighborhoods;
        private set => SetProperty(ref _filteredNeighborhoods, value);
    }

    public string ResultsTitleText
    {
        get => _resultsTitleText;
        private set => SetProperty(ref _resultsTitleText, value);
    }

    public void LoadNeighborhoods()
    {
        _allNeighborhoods = _neighborhoodService.SearchForCitizen(null, null, null, null);
        FilteredNeighborhoods = new ObservableCollection<NeighborhoodDto>(_allNeighborhoods);
        UpdateResultsTitle();
    }

    public void Search(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            FilteredNeighborhoods = new ObservableCollection<NeighborhoodDto>(_allNeighborhoods);
            UpdateResultsTitle();
            return;
        }

        string lowered = search.Trim().ToLower();
        var filtered = _allNeighborhoods.Where(n => MatchesSearch(n, lowered)).ToList();
        FilteredNeighborhoods = new ObservableCollection<NeighborhoodDto>(filtered);
        UpdateResultsTitle();
    }

    public void ApplyFilters(string? name, string? address, string? city, string? country)
    {
        var result = _neighborhoodService.SearchForCitizen(name, address, city, country);
        FilteredNeighborhoods = new ObservableCollection<NeighborhoodDto>(result);
        UpdateResultsTitle();
    }

    public void Reset()
    {
        FilteredNeighborhoods = new ObservableCollection<NeighborhoodDto>(_allNeighborhoods);
        UpdateResultsTitle();
    }

    private bool MatchesSearch(NeighborhoodDto n, string search)
    {
        if (n.Name.ToLower().Contains(search)) return true;
        if (n.Location.ToLower().Contains(search)) return true;
        return n.Streets.Any(s => s.StreetName.ToLower().Contains(search));
    }

    private void UpdateResultsTitle()
    {
        var title = ResourceHelper.Get("Browse_Title", "Browse Neighborhoods");
        var results = ResourceHelper.Get("Lbl_Results", "results");
        ResultsTitleText = $"{title} - {FilteredNeighborhoods.Count} {results}";
    }
}
