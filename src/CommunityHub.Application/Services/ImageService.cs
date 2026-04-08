using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Services;

public class ImageService
{
    private readonly ImageDbRepository _repository;

    public ImageService()
    {
        _repository = new ImageDbRepository();
    }

    public List<Image> GetForBuilding(long buildingId)
    {
        return _repository.GetByResource("building", buildingId);
    }

    // Kada dodate slike za kvartove:
    // public List<AppImage> GetForNeighborhood(long neighborhoodId)
    //     => _repository.GetByResource("neighborhood", neighborhoodId);
}
