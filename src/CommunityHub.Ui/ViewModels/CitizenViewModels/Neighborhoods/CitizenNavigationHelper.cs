using CommunityHub.Application.DependencyInjection;
using System.Windows;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.Helpers.Citizen;

namespace CommunityHub.Ui.Views.CitizenViews;

public static class CitizenNavigationHelper
{
    public static long? GetMembershipId(long userId)
    {
        NeighborhoodAccessRequestService s = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        long? nId = s.GetMembershipNeighborhoodId(userId);
        if (nId == null) { MsgHelper.Warn("Msg_NotMember", "Msg_Error"); return null; }
        return nId;
    }

    public static void NavigateToEvents(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new EventsPage(user, nId.Value, openMenuOnLoad: true).Show(); current.Close();
    }

    public static void NavigateToCitizens(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new NeighborhoodCitizensPage(user, nId.Value, openMenuOnLoad: true).Show(); current.Close();
    }

    public static void NavigateToMeetings(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new MeetingsPage(user, nId.Value, openMenuOnLoad: true).Show(); current.Close();
    }

    public static void NavigateToProfile(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(nId.Value) ?? "";
        new MyProfilePage(user, nId.Value, name, openMenuOnLoad: true).Show(); current.Close();
    }

    public static void NavigateToCityObjects(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new CityObjectsPage(user, nId.Value, openMenuOnLoad: true).Show(); current.Close();
    }

    public static void NavigateToBudget(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new BudgetPage(user, nId.Value, openMenuOnLoad: true).Show(); current.Close();
    }

    public static void NavigateToCoordinatorReviews(User user, long neighborhoodId, Window current)
    {
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        var (coordinatorId, coordinatorName) = ns.GetCoordinatorInfo(neighborhoodId);
        new CoordinatorReviewsPage(user, neighborhoodId, coordinatorId, coordinatorName, openMenuOnLoad: true).Show();
        current.Close();
    }
    public static void NavigateToEntry(NavigationEntry entry, Window current)
    {
        Window? next = entry.PageKey switch
        {
            "Neighborhoods" => new BrowseNeighborhoodPage(entry.User),
            "MyRequests" => new MyRequestsPage(entry.User),
            "Events" => new EventsPage(entry.User, entry.NeighborhoodId ?? GetMembershipId(entry.User.Id) ?? 0),
            "Citizens" => new NeighborhoodCitizensPage(entry.User, entry.NeighborhoodId ?? GetMembershipId(entry.User.Id) ?? 0),
            "Meetings" => new MeetingsPage(entry.User, entry.NeighborhoodId ?? GetMembershipId(entry.User.Id) ?? 0),
            "CityObjects" => new CityObjectsPage(entry.User, entry.NeighborhoodId ?? GetMembershipId(entry.User.Id) ?? 0),
            "CoordinatorReviews" => CreateCoordinatorReviewsPage(entry),
            "Budget" => new BudgetPage(entry.User, entry.NeighborhoodId ?? GetMembershipId(entry.User.Id) ?? 0),
            "Profile" => CreateProfilePage(entry),
            _ => null
        };

        if (next == null) return;
        next.Show();
        current.Close();
    }

    private static Window CreateCoordinatorReviewsPage(NavigationEntry entry)
    {
        long nId = entry.NeighborhoodId ?? GetMembershipId(entry.User.Id) ?? 0;
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        var (coordinatorId, coordinatorName) = ns.GetCoordinatorInfo(nId);
        return new CoordinatorReviewsPage(entry.User, nId, coordinatorId, coordinatorName);
    }

    private static Window CreateProfilePage(NavigationEntry entry)
    {
        long? nId = entry.NeighborhoodId ?? GetMembershipId(entry.User.Id);
        if (nId == null) return new BrowseNeighborhoodPage(entry.User);
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(nId.Value) ?? "";
        return new MyProfilePage(entry.User, nId.Value, name);
    }
}
