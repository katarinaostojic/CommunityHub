using CommunityHub.Ui.Helpers.Citizen;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class NeighborhoodCitizensViewModel : BaseViewModel
{
    private readonly TrustRecordService _service;
    private readonly long _neighborhoodId;

    private ObservableCollection<TrustRecordViewModel> _citizens = new();
    private string _resultsText = string.Empty;

    public NeighborhoodCitizensViewModel(TrustRecordService service, long neighborhoodId)
    {
        _service = service;
        _neighborhoodId = neighborhoodId;
        LoadCitizens();
        LanguageManager.LanguageChanged += LoadCitizens;
    }

    public ObservableCollection<TrustRecordViewModel> Citizens
    {
        get => _citizens;
        private set => SetProperty(ref _citizens, value);
    }

    public string ResultsText
    {
        get => _resultsText;
        private set => SetProperty(ref _resultsText, value);
    }

    private void LoadCitizens()
    {
        var items = _service.GetByNeighborhood(_neighborhoodId)
            .Select(dto => new TrustRecordViewModel(dto))
            .ToList();

        Citizens = new ObservableCollection<TrustRecordViewModel>(items);
        ResultsText = $"{ResourceHelper.Get("Citizens_Title", "Citizens")}: {items.Count}";
    }
}
