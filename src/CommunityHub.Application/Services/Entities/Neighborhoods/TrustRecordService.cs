using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain.Entities;
using CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;
using CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Mappings.Neighborhoods;

namespace CommunityHub.Application.Services.Entities.Neighborhoods;

public class TrustRecordService
{
    private readonly ITrustRecordRepository _repository;
    private readonly INeighborhoodMembershipRepository _membershipRepository;

    public TrustRecordService(
        ITrustRecordRepository repository,
        INeighborhoodMembershipRepository membershipRepository)
    {
        _repository = repository;
        _membershipRepository = membershipRepository;
    }

    public TrustRecordDto GetByCitizen(long citizenId, long neighborhoodId, string citizenFullName)
    {
        TrustRecord record = _repository.GetByCitizen(citizenId, neighborhoodId);
        return record.ToDto(citizenFullName);
    }

    public List<TrustRecordDto> GetByNeighborhood(long neighborhoodId)
    {
        var memberships = _membershipRepository.GetByNeighborhood(neighborhoodId);
        var records = _repository.GetByNeighborhood(neighborhoodId);

        return records.Select(r =>
        {
            var membership = memberships.FirstOrDefault(m => m.Citizen.Id == r.CitizenId);
            string fullName = membership != null
                ? $"{membership.Citizen.Name} {membership.Citizen.Surname}"
                : "Unknown";
            return r.ToDto(fullName);
        }).ToList();
    }
}
