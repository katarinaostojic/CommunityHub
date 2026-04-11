using CommunityHub.Application.Database;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;
using System.Data;
using System.Text;

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

    public AccessRequestResult RequestAccess(User citizen, Neighborhood neighborhood)
    {
        if (_repository.HasExistingPendingRequest(citizen, neighborhood))
            return AccessRequestResult.AlreadyPending;

        string userAddress = GetEffectiveUserAddress(citizen.Id, citizen.Address);

        bool addressMatches = false;

        if (!string.IsNullOrWhiteSpace(userAddress))
        {
            addressMatches = AddressMatchesNeighborhood(userAddress, neighborhood);
        }

        if (addressMatches)
        {
            if (_repository.HasMembership(citizen.Id))
                return AccessRequestResult.AlreadyMember;

            NeighborhoodAccessRequest request = new NeighborhoodAccessRequest(
                0,
                citizen,
                neighborhood,
                DateTime.UtcNow,
                RequestStatus.Approved,
                null
            );

            _repository.CreateMembership(request);
            return AccessRequestResult.Granted;
        }

        _repository.Create(citizen, neighborhood);
        return AccessRequestResult.RequestCreated;
    }

    private string GetEffectiveUserAddress(long userId, string? currentAddress)
    {
        if (!string.IsNullOrWhiteSpace(currentAddress))
            return currentAddress;

        using IDbConnection connection = PostgresConnection.CreateConnection();

        IDbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT address FROM users WHERE id = @id";

        IDbDataParameter idParam = command.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = userId;
        command.Parameters.Add(idParam);

        object? result = command.ExecuteScalar();

        if (result == null || result == DBNull.Value)
            return string.Empty;

        return result.ToString() ?? string.Empty;
    }

    private bool AddressMatchesNeighborhood(string fullAddress, Neighborhood neighborhood)
    {
        if (string.IsNullOrWhiteSpace(fullAddress))
            return false;

        if (neighborhood.Streets == null || !neighborhood.Streets.Any())
            return false;

        if (!TryParseAddress(fullAddress, out string userStreet, out int userNumber))
            return false;

        return neighborhood.Streets.Any(street =>
            Normalize(street.StreetName) == userStreet &&
            userNumber >= street.StartNumber &&
            userNumber <= street.EndNumber
        );
    }

    private bool TryParseAddress(string fullAddress, out string streetName, out int streetNumber)
    {
        streetName = string.Empty;
        streetNumber = 0;

        if (string.IsNullOrWhiteSpace(fullAddress))
            return false;

        string normalized = Normalize(fullAddress);

        int firstDigitIndex = -1;
        for (int i = 0; i < normalized.Length; i++)
        {
            if (char.IsDigit(normalized[i]))
            {
                firstDigitIndex = i;
                break;
            }
        }

        if (firstDigitIndex == -1)
            return false;

        string streetPart = normalized[..firstDigitIndex].Trim().Trim(',', '.', '-', '/');
        string numberPart = new string(normalized[firstDigitIndex..].TakeWhile(char.IsDigit).ToArray());

        if (string.IsNullOrWhiteSpace(streetPart))
            return false;

        if (!int.TryParse(numberPart, out streetNumber))
            return false;

        streetName = streetPart;
        return true;
    }

    private string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        string result = value.Trim().ToLowerInvariant();

        result = result
            .Replace("š", "s")
            .Replace("đ", "d")
            .Replace("č", "c")
            .Replace("ć", "c")
            .Replace("ž", "z");

        result = result
            .Replace("ulica", " ")
            .Replace("ul.", " ")
            .Replace("ul ", " ");

        StringBuilder sb = new StringBuilder();

        foreach (char c in result)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                sb.Append(c);
        }

        string cleaned = sb.ToString();

        while (cleaned.Contains("  "))
            cleaned = cleaned.Replace("  ", " ");

        return cleaned.Trim();
    }
}