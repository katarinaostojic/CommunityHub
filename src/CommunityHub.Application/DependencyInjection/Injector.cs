using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Database.Repositories.Ads;
using CommunityHub.Application.Database.Repositories.Buildings;
using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads.AdRepositoryInterfaces;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Application.Services.Shared;


namespace CommunityHub.Application.DependencyInjection;

public static class Injector
{
    private static readonly Dictionary<Type, object> _implementations = new()
    {
        {
            typeof(BuildingService),
            new BuildingService(
                new BuildingDbRepository(new ImageDbRepository()),
                new ImageDbRepository())
        },
        {
            typeof(BuildingAccessRequestService),
            new BuildingAccessRequestService(
                new BuildingAccessRequestDbRepository(),
                new BuildingMembershipDbRepository(),
                new BuildingDbRepository(new ImageDbRepository()))
        },
        {
            typeof(BuildingMembershipService),
            new BuildingMembershipService(
                new BuildingMembershipDbRepository())
        },
        {
            typeof(CityService),
            new CityService(
                new CityDbRepository())
        },
        {
            typeof(CountryService),
            new CountryService(
                new CountryDbRepository())
        },
        {
            typeof(AdService),
            new AdService(
                new AdDbRepository(),
                new AdSlotDbRepository(),
                new AdNotificationDbRepository())
        },
        {
            typeof(CommonRoomService),
            new CommonRoomService(
                new CommonRoomDbRepository(),
                new BuildingDbRepository(new ImageDbRepository()))
        },
        {
            typeof(NeighborhoodService),
            new NeighborhoodService(
                new NeighborhoodDbRepository())
        },
        {
            typeof(NeighborhoodAccessRequestService),
            new NeighborhoodAccessRequestService(
            new NeighborhoodAccessRequestDbRepository(),
            new NeighborhoodDbRepository())
        },
        {
            typeof(EventService),
            new EventService(
                new EventDbRepository())
        },
        {

            typeof(NeighborhoodMembershipService),
            new NeighborhoodMembershipService(
                new NeighborhoodMembershipDbRepository())
        },
        {
            typeof(MeetingService),
            new MeetingService(new MeetingDbRepository())
        },
        {
            typeof(StatisticsService),
            new StatisticsService(new TrustRecordDbRepository())
        },
        { 

            typeof(CommonRoomRequestService),
            new CommonRoomRequestService(
                new CommonRoomRequestDbRepository(),
                new CommonRoomDbRepository())
        },
        {
            typeof(ForumService),
            new ForumService(new ForumDbRepository())
        },
    };

    public static T CreateInstance<T>()
    {
        Type type = typeof(T);
        if (_implementations.TryGetValue(type, out object? implementation))
        {
            return (T)implementation;
        }
        throw new ArgumentException($"No implementation registered for type {type.FullName}");
    }
}