using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Database.Repositories.Ads;
using CommunityHub.Application.Database.Repositories.Buildings;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Ads;
using CommunityHub.Application.Domain.Buildings;
using CommunityHub.Application.Domain.Buildings.BuildingRepositoryInterfaces;
using CommunityHub.Application.Services.Ads;
using CommunityHub.Application.Services.Buildings;

namespace CommunityHub.Application.Services;

public static class ServiceFactory
{
    public static BuildingService CreateBuildingService()
    {
        IImageRepository imageRepository = new ImageDbRepository();
        IBuildingRepository buildingRepository = new BuildingDbRepository(imageRepository);
        return new BuildingService(buildingRepository, imageRepository);
    }

    public static BuildingAccessRequestService CreateBuildingAccessRequestService()
    {
        IBuildingAccessRequestRepository requestRepository = new BuildingAccessRequestDbRepository();
        IBuildingMembershipRepository membershipRepository = new BuildingMembershipDbRepository();
        return new BuildingAccessRequestService(requestRepository, membershipRepository);
    }

    public static BuildingMembershipService CreateBuildingMembershipService()
    {
        IBuildingMembershipRepository membershipRepository = new BuildingMembershipDbRepository();
        return new BuildingMembershipService(membershipRepository);
    }

    public static CityService CreateCityService()
    {
        ICityRepository cityRepository = new CityDbRepository();
        return new CityService(cityRepository);
    }

    public static CountryService CreateCountryService()
    {
        ICountryRepository countryRepository = new CountryDbRepository();
        return new CountryService(countryRepository);
    }

    public static AdService CreateAdService()
    {
        IAdRepository adRepository = new AdDbRepository();
        IAdSlotRepository adSlotRepository = new AdSlotDbRepository();
        return new AdService(adRepository, adSlotRepository);
    }
}