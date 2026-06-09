using CommunityHub.Application.Domain.Entities.Buildings.Ads;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.DTOs.Buildings.Ads;

namespace CommunityHub.Application.Services.Entities.Buildings.Ads;

public class AdCreationValidator
{
    private readonly IBuildingMembershipRepository _membershipRepository;

    public AdCreationValidator(IBuildingMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public void Validate(CreateAdDto request)
    {
        EnsureTenantHasBuildingMembership(request.Author.Id, request.BuildingId);
        ValidateDescription(request.Description);
        ValidateDateRange(request.DateFrom, request.DateTo);
    }

    private void EnsureTenantHasBuildingMembership(long tenantId, long buildingId)
    {
        if (!_membershipRepository.Exists(tenantId, buildingId))
            throw new InvalidOperationException("Tenant is not a member of this building.");
    }

    private static void ValidateDescription(string description)
    {
        string? validationError = Ad.ValidateDescription(description);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }

    private static void ValidateDateRange(DateOnly dateFrom, DateOnly dateTo)
    {
        string? validationError = Ad.ValidateDateRange(dateFrom, dateTo);

        if (validationError != null)
            throw new InvalidOperationException(validationError);
    }
}