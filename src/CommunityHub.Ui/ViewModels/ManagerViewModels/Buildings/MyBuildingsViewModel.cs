using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.DependencyInjection;
using System.Collections.ObjectModel;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings;

public class MyBuildingsViewModel : BaseViewModel
{
    private readonly BuildingService _buildingService;
    private readonly long _managerId;
    private List<BuildingDto> _allBuildings = new();
    private int _currentPage = 1;
    private const int PageSize = 6;

    private ObservableCollection<BuildingDto> _buildings = new();
    public ObservableCollection<BuildingDto> Buildings
    {
        get => _buildings;
        private set => SetProperty(ref _buildings, value);
    }

    private bool _hasPreviousPage;
    public bool HasPreviousPage
    {
        get => _hasPreviousPage;
        private set => SetProperty(ref _hasPreviousPage, value);
    }

    private bool _hasNextPage;
    public bool HasNextPage
    {
        get => _hasNextPage;
        private set => SetProperty(ref _hasNextPage, value);
    }

    private string _pageIndicator = string.Empty;
    public string PageIndicator
    {
        get => _pageIndicator;
        private set => SetProperty(ref _pageIndicator, value);
    }

    public MyBuildingsViewModel(long managerId)
    {
        _managerId = managerId;
        _buildingService = Injector.CreateInstance<BuildingService>();
        LoadBuildings();
    }

    public void LoadBuildings()
    {
        _allBuildings = _buildingService.GetAllByManager(_managerId);
        _currentPage = 1;
        UpdatePage();
    }

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

    public BuildingDto? GetBuildingById(long id)
    {
        return _buildingService.GetById(id);
    }

    private void UpdatePage()
    {
        int totalPages = Math.Max(1, (int)Math.Ceiling(_allBuildings.Count / (double)PageSize));
        PageIndicator = $"Page {_currentPage} / {totalPages}";
        HasPreviousPage = _currentPage > 1;
        HasNextPage = _currentPage < totalPages;

        Buildings = new ObservableCollection<BuildingDto>(
            _allBuildings
                .Skip((_currentPage - 1) * PageSize)
                .Take(PageSize)
        );
    }
}