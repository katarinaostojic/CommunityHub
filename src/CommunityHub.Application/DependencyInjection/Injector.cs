using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Database.Repositories.Ads;
using CommunityHub.Application.Database.Repositories.Buildings;
using CommunityHub.Application.Database.Repositories.Buildings.CommonRooms;
using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Application.Services.Buildings;
using CommunityHub.Application.Services.Buildings.CommonRooms;
using CommunityHub.Application.Services.Neighborhoods;
using CommunityHub.Application.Services.Shared;

namespace CommunityHub.Application.DependencyInjection;

public static class Injector
{
    private static readonly ImageDbRepository _imageRepository = new();
    private static readonly AdDbRepository _adRepository = new();
    private static readonly AdSlotDbRepository _adSlotRepository = new();
    private static readonly AdNotificationDbRepository _adNotificationRepository = new();
    private static readonly CommonRoomDbRepository _commonRoomRepository = new();
    private static readonly CommonRoomRequestDbRepository _commonRoomRequestRepository = new();

    private static readonly AdSlotBookingService _adSlotBookingService = new(
        _adRepository,
        _adSlotRepository,
        _adNotificationRepository);

    private static readonly CommonRoomRequestAvailabilityService _commonRoomRequestAvailabilityService = new(
        _commonRoomRepository);

    private static readonly CommonRoomRequestApprovalService _commonRoomRequestApprovalService = new(
        _commonRoomRequestRepository,
        _commonRoomRepository,
        _commonRoomRequestAvailabilityService);

    private static readonly Dictionary<Type, object> _implementations = new()
    {
        {
            typeof(BuildingService),
            new BuildingService(
                new BuildingDbRepository(_imageRepository),
                _imageRepository)
        },
        {
            typeof(BuildingAccessRequestService),
            new BuildingAccessRequestService(
                new BuildingAccessRequestDbRepository(),
                new BuildingMembershipDbRepository(),
                new BuildingDbRepository(_imageRepository))
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
            typeof(AdSlotBookingService),
            _adSlotBookingService
        },
        {
            typeof(AdNotificationService),
            new AdNotificationService(
                _adNotificationRepository)
        },
        {
            typeof(AdStatisticsService),
            new AdStatisticsService(
                _adRepository,
                _adSlotRepository)
        },
        {
            typeof(AdService),
            new AdService(
                _adRepository,
                _adSlotBookingService)
        },
        {
            typeof(CommonRoomService),
            new CommonRoomService(
                _commonRoomRepository,
                new BuildingDbRepository(_imageRepository))
        },
        {
            typeof(CommonRoomRequestAvailabilityService),
            _commonRoomRequestAvailabilityService
        },
        {
            typeof(CommonRoomRequestApprovalService),
            _commonRoomRequestApprovalService
        },
        {
            typeof(CommonRoomRequestService),
            new CommonRoomRequestService(
                _commonRoomRequestRepository,
                _commonRoomRequestApprovalService)
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
            new MeetingService(
                new MeetingDbRepository())
        },
        {
            typeof(StatisticsService),
            new StatisticsService(
                new TrustRecordDbRepository())
        },
        {
            typeof(ForumService),
            new ForumService(
                new ForumDbRepository())
        },
        {
            typeof(TrustRecordService),
            new TrustRecordService(
                new TrustRecordDbRepository(),
                new NeighborhoodMembershipDbRepository())
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