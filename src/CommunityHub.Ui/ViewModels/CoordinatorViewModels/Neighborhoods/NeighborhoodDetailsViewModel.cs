using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels;

public class NeighborhoodDetailsViewModel : BaseViewModel
{
    private readonly NeighborhoodMembershipService _membershipService;

    private ObservableCollection<NeighborhoodMembershipDto> _members = new();

    public NeighborhoodDetailsViewModel(NeighborhoodMembershipService membershipService, long neighborhoodId)
    {
        _membershipService = membershipService;
        LoadMembers(neighborhoodId);
    }

    public ObservableCollection<NeighborhoodMembershipDto> Members
    {
        get => _members;
        private set => SetProperty(ref _members, value);
    }

    private void LoadMembers(long neighborhoodId)
    {
        var members = _membershipService.GetByNeighborhood(neighborhoodId);
        Members = new ObservableCollection<NeighborhoodMembershipDto>(members);
    }
}