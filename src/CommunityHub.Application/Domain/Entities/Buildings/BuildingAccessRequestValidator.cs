using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Application.Domain.Entities.Buildings;

public class BuildingAccessRequestValidator
{
    public void EnsureCanBeCreated(Building building, long tenantId, string unitNumber)
    {
        EnsureUnitNumberIsEntered(unitNumber);
        EnsureBuildingContainsUnit(building, unitNumber);
        EnsureNoPendingRequest(building, tenantId, unitNumber);
    }

    public bool HasPendingRequest(Building building, long tenantId, string unitNumber)
    {
        return building.AccessRequests.Any(request =>
            IsPendingRequestForUnit(request, tenantId, unitNumber));
    }

    private static void EnsureUnitNumberIsEntered(string unitNumber)
    {
        if (string.IsNullOrWhiteSpace(unitNumber))
            throw new InvalidOperationException("Apartment number is required.");
    }

    private static void EnsureBuildingContainsUnit(Building building, string unitNumber)
    {
        if (!building.ContainsUnit(unitNumber))
            throw new InvalidOperationException("Apartment does not exist in this building.");
    }

    private void EnsureNoPendingRequest(Building building, long tenantId, string unitNumber)
    {
        if (HasPendingRequest(building, tenantId, unitNumber))
            throw new InvalidOperationException("You already have a pending request for this apartment.");
    }

    private static bool IsPendingRequestForUnit(
        BuildingAccessRequest request,
        long tenantId,
        string unitNumber)
    {
        return request.Tenant.Id == tenantId
            && request.UnitNumber == unitNumber
            && request.Status == RequestStatus.PendingApproval;
    }
}