using System.Windows.Controls;
using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class NeighborhoodDetailsPage : Page
{
    private readonly Neighborhood _neighborhood;
    private readonly NeighborhoodMembershipDbRepository _membershipRepository = new();

    public NeighborhoodDetailsPage(Neighborhood neighborhood)
    {
        InitializeComponent();
        _neighborhood = neighborhood;
        NeighborhoodNameText.Text = neighborhood.Name;
        LoadMembers();
    }

    private void LoadMembers()
    {
        var memberships = _membershipRepository.GetByNeighborhood(_neighborhood.Id);
        MembersItemsControl.ItemsSource = memberships.Select(m => new MemberDisplay(m)).ToList();
    }

    private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(new MyDistrictsPage(_neighborhood.CoordinatorId), "My Districts");
    }

    private class MemberDisplay
    {
        private readonly NeighborhoodMembership _membership;

        public MemberDisplay(NeighborhoodMembership membership)
        {
            _membership = membership;
        }

        public string FullName => $"{_membership.Citizen.Name} {_membership.Citizen.Surname}";
        public string Address => _membership.Citizen.Address ?? "No address";
        public string JoinedAt => $"Joined: {_membership.JoinedAt:dd.MM.yyyy}";
    }
    private void MeetingsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingsPage(_neighborhood.CoordinatorId, _neighborhood.Id), "Meetings");
    }
}
