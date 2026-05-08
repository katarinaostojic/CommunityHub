using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

public class BrowseBuildingsViewModel : BaseViewModel
{
    private readonly BuildingService _buildingService;
    private const int PageSize = 3;

    private List<Building> _allBuildings = new();
    private ObservableCollection<Building> _currentPageBuildings = new();
    private int _currentPage = 1;
    private string _pageLabelText = string.Empty;
    private bool _hasPreviousPage;
    private bool _hasNextPage;

    public BrowseBuildingsViewModel(BuildingService buildingService)
    {
        _buildingService = buildingService;
        Search(null, null, null, null);
    }

    public ObservableCollection<Building> CurrentPageBuildings
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

    public Building? GetBuildingById(long id) => _buildingService.GetById(id);

    public void NextPage()
    {
        if (!HasNextPage) return;
        _currentPage++;
        UpdatePage();
    }

    public void PreviousPage()
    {
        if (!HasPreviousPage) return;
        _currentPage--;
        UpdatePage();
    }

    private void UpdatePage()
    {
        int totalPages = Math.Max(1, (int)Math.Ceiling(_allBuildings.Count / (double)PageSize));

        PageLabelText = $"Page {_currentPage} of {totalPages}";
        HasPreviousPage = _currentPage > 1;
        HasNextPage = _currentPage < totalPages;

        CurrentPageBuildings = new ObservableCollection<Building>(
            _allBuildings
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize));
    }
}