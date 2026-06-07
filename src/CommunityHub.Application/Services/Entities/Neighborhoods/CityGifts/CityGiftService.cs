using CommunityHub.Application.Domain.Entities.Neighborhoods.CityGifts;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods.CityGifts;
using CommunityHub.Application.DTOs.Neighborhoods.CityGifts;

namespace CommunityHub.Application.Services.Entities.Neighborhoods.CityGifts;

public class CityGiftService
{
    private readonly ICityGiftRepository _repository;
    private readonly INeighborhoodRepository _neighborhoodRepository;

    public CityGiftService(ICityGiftRepository repository,
        INeighborhoodRepository neighborhoodRepository)
    {
        _repository = repository;
        _neighborhoodRepository = neighborhoodRepository;
    }

    public List<CityGiftDto> GetAll(long coordinatorId, long neighborhoodId)
    {
        var gifts = _repository.GetAll();
        var coordinatorNeighborhoodIds = _neighborhoodRepository.GetByCoordinator(coordinatorId)
            .Select(n => n.Id).ToList();

        return gifts.Select(g =>
        {
            var applications = _repository.GetApplications(g.Id);
            g.SetApplications(applications);

            string? awardedName = null;
            if (g.AwardedNeighborhoodId.HasValue)
            {
                var n = _neighborhoodRepository.GetById(g.AwardedNeighborhoodId.Value);
                awardedName = n?.Name;
            }

            bool coordinatorAlreadyApplied = !g.CanApply(neighborhoodId, coordinatorNeighborhoodIds)
                && !g.HasApplied(neighborhoodId)
                && !g.IsAwarded
                && !g.IsDeadlinePassed;

            return new CityGiftDto(
                g.Id, g.Amount,
                g.Deadline.ToString("dd.MM.yyyy."),
                g.IsAwarded,
                g.IsDeadlinePassed,
                g.HasApplied(neighborhoodId),
                g.CanApply(neighborhoodId, coordinatorNeighborhoodIds),
                awardedName,
                coordinatorAlreadyApplied);
        }).ToList();
    }

    public (bool success, string? error) Apply(long cityGiftId, long coordinatorId, long neighborhoodId)
    {
        CityGift? gift = _repository.GetById(cityGiftId);
        if (gift == null) return (false, "Gift not found.");

        var applications = _repository.GetApplications(cityGiftId);
        gift.SetApplications(applications);

        var coordinatorNeighborhoodIds = _neighborhoodRepository.GetByCoordinator(coordinatorId)
            .Select(n => n.Id).ToList();

        if (!gift.CanApply(neighborhoodId, coordinatorNeighborhoodIds))
            return (false, "Cannot apply for this gift.");

        _repository.Apply(cityGiftId, coordinatorId, neighborhoodId);
        return (true, null);
    }

    public void ProcessExpiredGifts()
    {
        var gifts = _repository.GetAll();
        foreach (var gift in gifts)
        {
            if (!gift.IsDeadlinePassed || gift.IsAwarded) continue;

            var applications = _repository.GetApplications(gift.Id);
            gift.SetApplications(applications);

            var neighborhoodIds = applications.Select(a => a.NeighborhoodId).ToList();
            if (!neighborhoodIds.Any()) continue;

            var budgets = _repository.GetBudgetsForNeighborhoods(neighborhoodIds);
            long? winnerId = gift.DetermineWinner(budgets);

            if (winnerId == null) continue;

            gift.Award(winnerId.Value);
            _repository.Award(gift.Id, winnerId.Value);

            int categoryCount = 6;
            decimal amountPerCategory = gift.AmountPerCategory(categoryCount);
            _repository.AddDonationToCategories(winnerId.Value, amountPerCategory);
        }
    }
}