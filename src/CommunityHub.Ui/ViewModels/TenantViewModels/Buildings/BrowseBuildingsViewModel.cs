using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Interfaces.Buildings;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings;

public class BrowseBuildingsViewModel : BaseViewModel
{
    private readonly IBuildingService _buildingService;
    private const int BuildingsPerPage = 3;

    private List<BuildingDto> _allBuildings = new();
    private ObservableCollection<BuildingDto> _currentPageBuildings = new();
    private int _currentPage = 1;
    private string _pageLabelText = string.Empty;
    private bool _hasPreviousPage;
    private bool _hasNextPage;

    public BrowseBuildingsViewModel(IBuildingService buildingService)
    {
        _buildingService = buildingService;
        Search(null, null, null, null);
    }

    public ObservableCollection<BuildingDto> CurrentPageBuildings
    {
        get => _currentPageBuildings;
        private set => SetProperty(ref _currentPageBuildings, value);
    }

    public string PageLabelText
    {
        get => _pageLabelText;
        private set => SetProperty(ref _pageLabelText, value);
    }

    public bool HasPreviousPage
    {
        get => _hasPreviousPage;
        private set => SetProperty(ref _hasPreviousPage, value);
    }

    public bool HasNextPage
    {
        get => _hasNextPage;
        private set => SetProperty(ref _hasNextPage, value);
    }

    public void Search(string? street, string? neighborhood, string? city, string? country)
    {
        _allBuildings = _buildingService.Search(street, neighborhood, city, country);
        _currentPage = 1;
        UpdatePage();
    }

    public BuildingDto? GetFullBuildingDto(long id)
    {
        return _buildingService.GetById(id);
    }

    public void NextPage()
    {
        if (!HasNextPage)
            return;

        _currentPage++;
        UpdatePage();
    }

    public void PreviousPage()
    {
        if (!HasPreviousPage)
            return;

        _currentPage--;
        UpdatePage();
    }

    private void UpdatePage()
    {
        int totalPages = Math.Max(1, (int)Math.Ceiling(_allBuildings.Count / (double)BuildingsPerPage));

        PageLabelText = $"Page {_currentPage} of {totalPages}";
        HasPreviousPage = _currentPage > 1;
        HasNextPage = _currentPage < totalPages;

        CurrentPageBuildings = new ObservableCollection<BuildingDto>(
            _allBuildings
                .Skip((_currentPage - 1) * BuildingsPerPage)
                .Take(BuildingsPerPage));
    }
}