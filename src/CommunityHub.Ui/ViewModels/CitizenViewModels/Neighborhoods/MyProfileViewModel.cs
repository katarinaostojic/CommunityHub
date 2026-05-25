using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class MyProfileViewModel : BaseViewModel
{
    private readonly TrustRecordService _service;
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly string _neighborhoodName;

    private TrustRecordViewModel? _trustRecord;
    private string _resultsText = string.Empty;

    public MyProfileViewModel(TrustRecordService service, User user, long neighborhoodId, string neighborhoodName)
    {
        _service = service;
        _user = user;
        _neighborhoodId = neighborhoodId;
        _neighborhoodName = neighborhoodName;
        LoadProfile();
    }

    public TrustRecordViewModel? TrustRecord
    {
        get => _trustRecord;
        private set => SetProperty(ref _trustRecord, value);
    }

    public string Username => _user.Username;
    public string NeighborhoodName => _neighborhoodName;

    private void LoadProfile()
    {
        string fullName = $"{_user.Name} {_user.Surname}";
        var dto = _service.GetByCitizen(_user.Id, _neighborhoodId, fullName);
        TrustRecord = new TrustRecordViewModel(dto);
    }
    public string PieChartData => GeneratePieChart();

    private string GeneratePieChart()
    {
        int organized = TrustRecord?.EventsOrganized ?? 0;
        int volunteered = TrustRecord?.EventsVolunteered ?? 0;
        int total = organized + volunteered;
        if (total == 0) return "0,0,0,0,0,0";
        double angle = (organized / (double)total) * 360.0;
        return angle.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
}
