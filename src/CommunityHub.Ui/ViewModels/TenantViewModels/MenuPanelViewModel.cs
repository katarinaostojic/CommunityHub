using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Application.Services.Interfaces.Buildings;

namespace CommunityHub.Ui.ViewModels.TenantViewModels;

public class MenuPanelViewModel
{
    private readonly IBuildingMembershipService _membershipService;

    public MenuPanelViewModel(IBuildingMembershipService membershipService)
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