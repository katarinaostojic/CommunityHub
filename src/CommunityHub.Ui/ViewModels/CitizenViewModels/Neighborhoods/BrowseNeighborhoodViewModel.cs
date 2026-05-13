using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.Services.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class BrowseNeighborhoodViewModel : BaseViewModel
{
    private readonly NeighborhoodService _neighborhoodService;

    private List<Neighborhood> _allNeighborhoods = new();
    private ObservableCollection<Neighborhood> _filteredNeighborhoods = new();
    private string _resultsTitleText = string.Empty;

    public BrowseNeighborhoodViewModel(NeighborhoodService neighborhoodService)
    {
        _neighborhoodService = neighborhoodService;
        LoadNeighborhoods();
    }

    public ObservableCollection<Neighborhood> FilteredNeighborhoods
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
        FilteredNeighborhoods = new ObservableCollection<Neighborhood>(_allNeighborhoods);
        UpdateResultsTitle();
    }

    public void Search(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            FilteredNeighborhoods = new ObservableCollection<Neighborhood>(_allNeighborhoods);
            UpdateResultsTitle();
            return;
        }

        string lowered = search.Trim().ToLower();
        var filtered = _allNeighborhoods.Where(n => n.MatchesSearch(lowered)).ToList();
        FilteredNeighborhoods = new ObservableCollection<Neighborhood>(filtered);
        UpdateResultsTitle();
    }

    public void ApplyFilters(string? name, string? address, string? city, string? country)
    {
        var result = _neighborhoodService.SearchForCitizen(name, address, city, country);
        FilteredNeighborhoods = new ObservableCollection<Neighborhood>(result);
        UpdateResultsTitle();
    }

    public void Reset()
    {
        FilteredNeighborhoods = new ObservableCollection<Neighborhood>(_allNeighborhoods);
        UpdateResultsTitle();
    }

    private void UpdateResultsTitle()
    {
        ResultsTitleText = $"Browse Neighborhood - {FilteredNeighborhoods.Count} results";
    }
}