using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Application.Mappings.Neighborhoods;

public static class NeighborhoodMappingExtensions
{
    public static NeighborhoodDto ToDto(this Neighborhood neighborhood)
    {
        return new NeighborhoodDto(
            id: neighborhood.Id,
            name: neighborhood.Name,
            description: neighborhood.Description,
            cityName: neighborhood.Location.CityName,
            countryName: neighborhood.Location.CountryName,
            coordinatorId: neighborhood.CoordinatorId,
            streets: neighborhood.Streets.Select(s => s.ToDto()).ToList(),
            imagePaths: neighborhood.Images.Select(i => i.Path).ToList()
        );
    }

    public static List<NeighborhoodDto> ToDtoList(this IEnumerable<Neighborhood> neighborhoods)
        => neighborhoods.Select(n => n.ToDto()).ToList();

    public static StreetDto ToDto(this Street street)
    {
        return new StreetDto(
            id: street.Id,
            streetName: street.StreetName,
            startNumber: street.StartNumber,
            endNumber: street.EndNumber
        );
    }

    public static ForumDto ToDto(this Forum forum, long currentCoordinatorId)
    {
        return new ForumDto(
            id: forum.Id,
            title: forum.Title,
            description: forum.Description,
            coordinatorId: forum.CoordinatorId,
            coordinatorFullName: $"{forum.CoordinatorName} {forum.CoordinatorSurname}",
            isClosed: forum.IsClosed,
            createdAt: forum.CreatedAt,
            isAuthor: forum.CoordinatorId == currentCoordinatorId
        );
    }

    public static List<ForumDto> ToDtoList(this IEnumerable<Forum> forums, long currentCoordinatorId)
        => forums.Select(f => f.ToDto(currentCoordinatorId)).ToList();

    public static ForumCommentDto ToDto(this ForumComment comment, long forumCoordinatorId)
    {
        return new ForumCommentDto(
            id: comment.Id,
            forumId: comment.ForumId,
            coordinatorId: comment.CoordinatorId,
            coordinatorFullName: $"{comment.CoordinatorName} {comment.CoordinatorSurname}",
            neighborhoodName: comment.NeighborhoodName,
            text: comment.Text,
            createdAt: comment.CreatedAt,
            likeCount: comment.LikeCount,
            dislikeCount: comment.DislikeCount,
            currentUserReaction: comment.CurrentUserReaction,
            isAuthor: comment.CoordinatorId == forumCoordinatorId
        );
    }

    public static List<ForumCommentDto> ToDtoList(this IEnumerable<ForumComment> comments, long forumCoordinatorId)
        => comments.Select(c => c.ToDto(forumCoordinatorId)).ToList();
}