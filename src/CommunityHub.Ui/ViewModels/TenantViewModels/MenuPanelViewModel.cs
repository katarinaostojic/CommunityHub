using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Entities.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

public class MenuPanelViewModel
{
    private readonly BuildingMembershipService _membershipService;

    public MenuPanelViewModel(BuildingMembershipService membershipService)
    {
        _membershipService = membershipService;
    }

    public List<BuildingMembershipDto> GetMemberships(long tenantId)
    {
        return _membershipService.GetByTenant(tenantId)
            .GroupBy(m => m.BuildingId)
            .Select(g => g.First())
            .ToList();
    }
}