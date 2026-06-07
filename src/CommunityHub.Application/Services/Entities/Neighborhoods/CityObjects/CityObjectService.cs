using CommunityHub.Application.Domain.Entities.Neighborhoods.CityObjects;
using CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;
using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.CityObjects;

namespace CommunityHub.Application.Services.Entities.Neighborhoods.CityObjects;

public class CityObjectService
{
    private readonly ICityObjectRepository _repository;

    public CityObjectService(ICityObjectRepository repository)
    {
        _repository = repository;
    }

    public List<CityObjectDto> GetAll()
    {
        return _repository.GetAll()
            .Select(co => new CityObjectDto(co.Id, co.Name, co.Description, co.VoteCount))
            .ToList();
    }

    // Vraća slobodan termin u opsegu, ili null ako ne postoji
    public SlotSuggestion? FindSlot(ReserveRequest request)
    {
        CityObject? cityObject = _repository.GetById(request.CityObjectId);
        if (cityObject == null) return null;

        List<CityObjectReservation> reservations = _repository.GetReservations(request.CityObjectId);
        cityObject.SetReservations(reservations);

        DateOnlyRange? slot = cityObject.FindFreeSlotInRange(
            request.RangeFrom, request.RangeTo, request.DurationDays);

        if (slot == null) return null;

        return new SlotSuggestion(slot.From, slot.To, isAlternative: false);
    }

    // Vraća alternativne termine van opsega
    public List<SlotSuggestion> FindAlternativeSlots(ReserveRequest request)
    {
        CityObject? cityObject = _repository.GetById(request.CityObjectId);
        if (cityObject == null) return new List<SlotSuggestion>();

        List<CityObjectReservation> reservations = _repository.GetReservations(request.CityObjectId);
        cityObject.SetReservations(reservations);

        return cityObject
            .FindAlternativeSlots(request.RangeFrom, request.RangeTo, request.DurationDays)
            .Select(r => new SlotSuggestion(r.From, r.To, isAlternative: true))
            .ToList();
    }

    // Rezerviše termin i resetuje glasove za taj objekat/kvart
    public void Reserve(long cityObjectId, long neighborhoodId, DateOnly dateFrom, DateOnly dateTo)
    {
        CityObject? cityObject = _repository.GetById(cityObjectId);
        if (cityObject == null) return;

        cityObject.Reserve(neighborhoodId, dateFrom, dateTo);

        CityObjectReservation newReservation = cityObject.Reservations.Last();
        _repository.AddReservation(newReservation);
        _repository.ResetVotes(cityObjectId, neighborhoodId);
    }

    public List<CitizenCityObjectDto> GetByNeighborhood(long neighborhoodId, long citizenId)
    {
        return _repository.GetByNeighborhood(neighborhoodId, citizenId)
            .Select(co => new CitizenCityObjectDto
            {
                Id = co.Id,
                Name = co.Name,
                Description = co.Description,
                VoteCount = co.VoteCount,
                LastVisit = co.LastVisit?.ToString("dd/MM/yyyy"),
                HasVoted = co.HasVoted
            }).ToList();
    }

    public void ToggleVote(long cityObjectId, long citizenId, long neighborhoodId)
    {
        if (_repository.HasVoted(cityObjectId, citizenId))
            _repository.RemoveVote(cityObjectId, citizenId);
        else
            _repository.AddVote(cityObjectId, citizenId, neighborhoodId);
    }

    public CityObjectStatisticsDto GetStatistics(long neighborhoodId, int? month, int? year)
    {
        var reservations = _repository.GetReservationsByNeighborhood(neighborhoodId, month, year);

        var visitCounts = reservations
            .GroupBy(r => r.CityObjectId)
            .Select(g => new CityObjectVisitCountDto
        {
            CityObjectId = g.Key,
            CityObjectName = GetCityObjectName(g.Key),
            VisitCount = g.Count()
        }).ToList();

        return new CityObjectStatisticsDto
        {
            TotalReservations = reservations.Count,
            VisitCounts = visitCounts
        };
    }

    public List<CityObjectReservationDto> GetReservationHistory(long cityObjectId, long neighborhoodId)
    {
        string cityObjectName = GetCityObjectName(cityObjectId);
        return _repository.GetReservationsByCityObject(cityObjectId, neighborhoodId)
            .Select(r => new CityObjectReservationDto
            {
                Id = r.Id,
                CityObjectId = r.CityObjectId,
                CityObjectName = cityObjectName,
                DateFrom = r.DateFrom.ToString("dd/MM/yyyy"),
                DateTo = r.DateTo.ToString("dd/MM/yyyy")
            }).ToList();
    }

    private string GetCityObjectName(long cityObjectId)
    {
        return _repository.GetById(cityObjectId)?.Name ?? "Unknown";
    }
    public List<CityObjectReservationDto> GetReservationHistoryFiltered(long cityObjectId, long neighborhoodId, int? month, int? year)
    {
        string cityObjectName = GetCityObjectName(cityObjectId);
        return _repository.GetReservationsByCityObject(cityObjectId, neighborhoodId)
            .Where(r => (!month.HasValue || r.DateFrom.Month == month) &&
                        (!year.HasValue || r.DateFrom.Year == year))
            .Select(r => new CityObjectReservationDto
            {
                Id = r.Id,
                CityObjectId = r.CityObjectId,
                CityObjectName = cityObjectName,
                DateFrom = r.DateFrom.ToString("dd/MM/yyyy"),
                DateTo = r.DateTo.ToString("dd/MM/yyyy")
            }).ToList();
    }

    public List<CityObjectDto> GetAllByNeighborhood(long neighborhoodId)
    {
        return _repository.GetAllByNeighborhood(neighborhoodId)
            .Select(co => new CityObjectDto(co.Id, co.Name, co.Description, co.VoteCount))
            .ToList();
    }
}
