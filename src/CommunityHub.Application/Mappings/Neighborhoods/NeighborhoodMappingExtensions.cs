using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;
using CommunityHub.Application.DTOs.Neighborhoods;
using static CommunityHub.Application.DTOs.Neighborhoods.NeighborhoodAccessRequestDto;

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
            imagePaths: neighborhood.Images.Select(i => i.Path).ToList()
        );
    }

    public static List<NeighborhoodDto> ToDtoList(this IEnumerable<Neighborhood> neighborhoods)
    {
        return neighborhoods.Select(n => n.ToDto()).ToList();
    }

    public static NeighborhoodMembershipDto ToDto(this NeighborhoodMembership membership)
    {
        return new NeighborhoodMembershipDto(
            id: membership.Id,
            citizenName: membership.Citizen.Name,
            citizenSurname: membership.Citizen.Surname,
            citizenAddress: membership.Citizen.Address,
            joinedAt: membership.JoinedAt
        );
    }

    public static List<NeighborhoodMembershipDto> ToDtoList(this IEnumerable<NeighborhoodMembership> memberships)
    {
        return memberships.Select(m => m.ToDto()).ToList();
    }
    public static NeighborhoodAccessRequestDto ToDto(this NeighborhoodAccessRequest request)
    {
        return new NeighborhoodAccessRequestDto(
            id: request.Id,
            citizenName: request.Citizen.Name,
            citizenSurname: request.Citizen.Surname,
            citizenAddress: request.Citizen.Address,
            neighborhoodName: request.Neighborhood.Name,
            createdAt: request.CreatedAt,
            status: request.Status,
            rejectionReason: request.RejectionReason
        );
    }

    public static List<NeighborhoodAccessRequestDto> ToDtoList(this IEnumerable<NeighborhoodAccessRequest> requests)
    {
        return requests.Select(r => r.ToDto()).ToList();
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
    {
        return forums.Select(f => f.ToDto(currentCoordinatorId)).ToList();
    }

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
    {
        return comments.Select(c => c.ToDto(forumCoordinatorId)).ToList();
    }
}