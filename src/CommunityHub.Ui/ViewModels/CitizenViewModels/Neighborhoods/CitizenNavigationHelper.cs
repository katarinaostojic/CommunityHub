using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.DependencyInjection;
using System.Windows;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;

namespace CommunityHub.Ui.Views.CitizenViews;

public static class CitizenNavigationHelper
{
    public static long? GetMembershipId(long userId)
    {
        NeighborhoodAccessRequestService s = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        long? nId = s.GetMembershipNeighborhoodId(userId);
        if (nId == null) MessageBox.Show("You are not a member of any neighborhood.");
        return nId;
    }

    public static void NavigateToEvents(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new EventsPage(user, nId.Value).Show(); current.Close();
    }

    public static void NavigateToCitizens(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new NeighborhoodCitizensPage(user, nId.Value).Show(); current.Close();
    }

    public static void NavigateToMeetings(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new MeetingsPage(user, nId.Value).Show(); current.Close();
    }

    public static void NavigateToProfile(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        NeighborhoodService ns = Injector.CreateInstance<NeighborhoodService>();
        string name = ns.GetNameById(nId.Value) ?? "";
        new MyProfilePage(user, nId.Value, name).Show(); current.Close();
    }
    public static void NavigateToCityObjects(User user, Window current)
    {
        long? nId = GetMembershipId(user.Id);
        if (nId == null) return;
        new CityObjectsPage(user, nId.Value).Show(); current.Close();
    }
}
