using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunityHub.Application.Services;

public class NeighborhoodAccessRequestService
{
    private readonly NeighborhoodAccessRequestDbRepository _repository;

    public NeighborhoodAccessRequestService()
    {
        _repository = new NeighborhoodAccessRequestDbRepository();
    }

    public List<NeighborhoodAccessRequest> GetAllByCitizen(long citizenId, string? status, bool sortDescending)
    {
        return _repository.GetAllByCitizen(citizenId, status, sortDescending);
    }

    public int CountByCitizenAndStatus(long citizenId, string? status)
    {
        return _repository.CountByCitizenAndStatus(citizenId, status);
    }

    public void Delete(long id)
    {
        _repository.Delete(id);
    }

    public void Create(User citizen, Neighborhood neighborhood)
    {
        _repository.Create(citizen, neighborhood);
    }

    public bool HasExistingPendingRequest(User citizen, Neighborhood neighborhood)
    {
        return _repository.HasExistingPendingRequest(citizen, neighborhood);
    }

    public void ApproveRequest(NeighborhoodAccessRequest request)
    {
        _repository.ApproveRequest(request.Id);
    }

    public void RejectRequest(long requestId, string? rejectionReason)
    {
        _repository.RejectRequest(requestId, rejectionReason);
    }

    public bool RequestAccess(User citizen, Neighborhood neighborhood)
    {
        if (_repository.HasMembership(citizen.Id))
            return false;

        if (_repository.HasExistingPendingRequest(citizen, neighborhood))
            return false;

        if (string.IsNullOrWhiteSpace(citizen.Address))
        {
            _repository.Create(citizen, neighborhood);
            return false;
        }

        string address = citizen.Address.Trim();

        Match matchNumber = Regex.Match(address, @"^(.*)\s+(\d+)$");
        if (!matchNumber.Success)
        {
            _repository.Create(citizen, neighborhood);
            return false;
        }

        string userStreet = NormalizeStreetName(matchNumber.Groups[1].Value);
        int userNumber = int.Parse(matchNumber.Groups[2].Value);

        bool addressMatches = neighborhood.Streets.Any(street =>
            NormalizeStreetName(street.StreetName) == userStreet &&
            userNumber >= street.StartNumber &&
            userNumber <= street.EndNumber);

        if (addressMatches)
        {
            var request = new NeighborhoodAccessRequest(
                0,
                citizen,
                neighborhood,
                DateTime.UtcNow,
                RequestStatus.Approved,
                null
            );

            _repository.CreateMembership(request);
            return true;
        }

        _repository.Create(citizen, neighborhood);
        return false;
    }

    private string NormalizeStreetName(string value)
    {
        string normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalized)
        {
            UnicodeCategory category = CharUnicodeInfo.GetUnicodeCategory(c);
            if (category != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString()
            .Replace("đ", "dj")
            .Normalize(NormalizationForm.FormC);
    }
    private readonly NeighborhoodDbRepository _neighborhoodRepository = new();

    public List<NeighborhoodAccessRequest> GetAllByCoordinator(long coordinatorId, string? status, bool sortDescending)
    {
        return _neighborhoodRepository.GetAllByCoordinator(coordinatorId, status, sortDescending);
    }

    public void ApproveRequestWithMembership(long requestId, long citizenId, long neighborhoodId)
    {
        _neighborhoodRepository.ApproveRequest(requestId, citizenId, neighborhoodId);
    }

    public void RejectRequestForCoordinator(long requestId, string? rejectionReason)
    {
        _neighborhoodRepository.RejectRequest(requestId, rejectionReason);
    }
}