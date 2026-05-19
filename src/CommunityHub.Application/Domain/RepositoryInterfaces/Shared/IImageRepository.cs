using CommunityHub.Application.Domain.Shared;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Shared;

public interface IImageRepository
{
    List<Image> GetByEntity(string entity, long entityId);
    Dictionary<long, List<Image>> GetByEntities(string entity, IEnumerable<long> entityIds);
    void SaveImage(string entity, long entityId, string path);
}