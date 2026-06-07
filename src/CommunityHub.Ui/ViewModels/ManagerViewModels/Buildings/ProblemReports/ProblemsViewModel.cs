using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.ProblemReports;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.ManagerViewModels.Buildings.ProblemReports;

public class ProblemsViewModel : BaseViewModel
{
    private readonly ProblemReportService _problemService;
    private readonly BuildingService _buildingService;
    private readonly long _managerId;

    private ObservableCollection<BuildingDto> _buildings = new();
    public ObservableCollection<BuildingDto> Buildings
    {
        get => _buildings;
        private set => SetProperty(ref _buildings, value);
    }

    private BuildingDto? _selectedBuilding;
    public BuildingDto? SelectedBuilding
    {
        get => _selectedBuilding;
        set
        {
            if (value == null) return;
            SetProperty(ref _selectedBuilding, value);
            LoadProblems();
        }
    }

    private ObservableCollection<ProblemReportRowViewModel> _problems = new();
    public ObservableCollection<ProblemReportRowViewModel> Problems
    {
        get => _problems;
        private set => SetProperty(ref _problems, value);
    }

    public ProblemsViewModel(long managerId)
    {
        _managerId = managerId;
        _problemService = Injector.CreateInstance<ProblemReportService>();
        _buildingService = Injector.CreateInstance<BuildingService>();
        LoadBuildings();
    }

    public void LoadBuildings()
    {
        var buildings = _buildingService.GetAllByManager(_managerId);
        Buildings = new ObservableCollection<BuildingDto>(buildings);
    }

    public void LoadProblems()
    {
        if (_selectedBuilding == null)
        {
            Problems = new ObservableCollection<ProblemReportRowViewModel>();
            return;
        }

        var reports = _problemService.GetByBuilding(_selectedBuilding.Id);
        Problems = new ObservableCollection<ProblemReportRowViewModel>(
            reports.Select((r, i) => new ProblemReportRowViewModel(r, i % 2 == 1)).ToList()
        );
    }

    public void MarkAsResolved(long reportId)
    {
        _problemService.MarkAsPotentiallySolved(reportId);
        LoadProblems();
    }
}