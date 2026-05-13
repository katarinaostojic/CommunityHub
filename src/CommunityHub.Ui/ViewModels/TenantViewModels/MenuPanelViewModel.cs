using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Services.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

public class MenuPanelViewModel
{
    private readonly BuildingMembershipService _membershipService;

    public MenuPanelViewModel(BuildingMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    public List<BuildingMembership> GetMemberships(long tenantId)
    {
        return _membershipService.GetByTenant(tenantId)
            .GroupBy(m => m.Building.Id)
            .Select(g => g.First())
            .ToList();
    }
}