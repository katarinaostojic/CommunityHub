using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class CityObjectsViewModel : BaseViewModel
{
    private readonly CityObjectService _service;
    private readonly long _neighborhoodId;
    private readonly long _citizenId;
    private ObservableCollection<CityObjectItemViewModel> _cityObjects = new();
    private string _resultsText = string.Empty;

    public CityObjectsViewModel(CityObjectService service, long neighborhoodId, long citizenId)
    {
        _service = service;
        _neighborhoodId = neighborhoodId;
        _citizenId = citizenId;
        LoadCityObjects();
    }

    public ObservableCollection<CityObjectItemViewModel> CityObjects
    {
        get => _cityObjects;
        private set => SetProperty(ref _cityObjects, value);
    }

    public string ResultsText
    {
        get => _resultsText;
        private set => SetProperty(ref _resultsText, value);
    }

    public void LoadCityObjects()
    {
        var items = _service.GetByNeighborhood(_neighborhoodId, _citizenId)
            .Select(dto => new CityObjectItemViewModel(dto))
            .ToList();
        CityObjects = new ObservableCollection<CityObjectItemViewModel>(items);
        ResultsText = $"Showing {items.Count} objects";
    }

    public void ToggleVote(long cityObjectId)
    {
        _service.ToggleVote(cityObjectId, _citizenId, _neighborhoodId);
        LoadCityObjects();
    }

    public CityObjectStatisticsDto GetStatistics(int? month, int? year)
        => _service.GetStatistics(_neighborhoodId, month, year);

    public List<CityObjectReservationDto> GetHistory(long cityObjectId)
        => _service.GetReservationHistory(cityObjectId, _neighborhoodId);
    public List<CityObjectReservationDto> GetHistoryById(long cityObjectId, int? month, int? year)
    {
        return _service.GetReservationHistoryFiltered(cityObjectId, _neighborhoodId, month, year);
    }
}
