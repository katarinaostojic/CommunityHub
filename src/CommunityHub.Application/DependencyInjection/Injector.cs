using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Database.Repositories.Ads;
using CommunityHub.Application.Database.Repositories.Buildings;
using CommunityHub.Application.Database.Repositories.Buildings.CommonRooms;
using CommunityHub.Application.Database.Repositories.Neighborhoods;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services.Entities.Ads;
using CommunityHub.Application.Services.Entities.Buildings;
using CommunityHub.Application.Services.Entities.Buildings.CommonRooms;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Application.Services.Entities.Shared;

namespace CommunityHub.Application.DependencyInjection;

public static class Injector
{
    private static readonly ImageDbRepository _imageRepository = new();

    private static readonly BuildingDetailsDbRepository _buildingDetailsRepository = new(
        _imageRepository);

    private static readonly BuildingDbRepository _buildingRepository = new(
        _buildingDetailsRepository);

    private static readonly AdDbRepository _adRepository = new();
    private static readonly AdSlotDbRepository _adSlotRepository = new();
    private static readonly AdNotificationDbRepository _adNotificationRepository = new();

    private static readonly CommonRoomDbRepository _commonRoomRepository = new();
    private static readonly CommonRoomRequestDbRepository _commonRoomRequestRepository = new();

    private static readonly AdExpirationService _adExpirationService = new(
        _adRepository);

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

    private static readonly CommonRoomRequestCommandService _commonRoomRequestCommandService = new(
    _commonRoomRequestRepository,
    _commonRoomRequestApprovalService);

    private static readonly Dictionary<Type, object> _implementations = new()
    {
        {
            typeof(BuildingService),
            new BuildingService(
                _buildingRepository,
                _imageRepository)
        },
        {
            typeof(BuildingAccessRequestService),
            new BuildingAccessRequestService(
                new BuildingAccessRequestDbRepository(),
                new BuildingMembershipDbRepository(),
                _buildingRepository)
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
            typeof(AdExpirationService),
            _adExpirationService
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
                _adSlotRepository,
                _adExpirationService)
        },
        {
            typeof(AdService),
            new AdService(
                _adRepository,
                _adSlotBookingService,
                _adExpirationService)
        },
        {
            typeof(CommonRoomService),
            new CommonRoomService(
                _commonRoomRepository,
                _buildingRepository)
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
            typeof(CommonRoomRequestCommandService),
            _commonRoomRequestCommandService
        },
        {
            typeof(CommonRoomRequestService),
            new CommonRoomRequestService(
                _commonRoomRequestRepository,
                _commonRoomRequestApprovalService,
                _commonRoomRequestCommandService)
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