using CommunityHub.Application.Domain.Entities.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

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
}
