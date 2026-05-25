namespace CommunityHub.Application.DTOs.Neighborhoods;

using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Entities.Neighborhoods;

public class NeighborhoodDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string CityName { get; init; }
    public string CountryName { get; init; }
    public long CoordinatorId { get; init; }
    public List<StreetDto> Streets { get; init; }
    public List<string> ImagePaths { get; init; }

    public NeighborhoodDto(long id, string name, string description,
        string cityName, string countryName, long coordinatorId,
        List<StreetDto> streets, List<string> imagePaths)
    {
        Id = id;
        Name = name;
        Description = description;
        CityName = cityName;
        CountryName = countryName;
        CoordinatorId = coordinatorId;
        Streets = streets;
        ImagePaths = imagePaths;
    }

    public string Location => $"{CityName}, {CountryName}";
}

public class StreetDto
{
    public long Id { get; init; }
    public string StreetName { get; init; }
    public int StartNumber { get; init; }
    public int EndNumber { get; init; }

    public StreetDto(long id, string streetName, int startNumber, int endNumber)
    {
        Id = id;
        StreetName = streetName;
        StartNumber = startNumber;
        EndNumber = endNumber;
    }

    public string Display => $"{StreetName} {StartNumber} - {EndNumber}";
}

public class ForumDto
{
    public long Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public long CoordinatorId { get; init; }
    public string CoordinatorFullName { get; init; } = string.Empty;
    public bool IsClosed { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsAuthor { get; init; }
    public int CommentsCount { get; init; }

    public string CreatedAtFormatted => $"{CreatedAt:dd.MM.yyyy.}";
}

public class ForumCommentDto
{
    public long Id { get; init; }
    public long ForumId { get; init; }
    public long CoordinatorId { get; init; }
    public string CoordinatorFullName { get; init; }
    public string NeighborhoodName { get; init; }
    public string Text { get; init; }
    public DateTime CreatedAt { get; init; }
    public int LikeCount { get; init; }
    public int DislikeCount { get; init; }
    public ReactionType? CurrentUserReaction { get; init; }
    public bool IsAuthor { get; init; }

    public ForumCommentDto(long id, long forumId, long coordinatorId, string coordinatorFullName,
        string neighborhoodName, string text, DateTime createdAt, int likeCount, int dislikeCount,
        ReactionType? currentUserReaction, bool isAuthor)
    {
        Id = id;
        ForumId = forumId;
        CoordinatorId = coordinatorId;
        CoordinatorFullName = coordinatorFullName;
        NeighborhoodName = neighborhoodName;
        Text = text;
        CreatedAt = createdAt;
        LikeCount = likeCount;
        DislikeCount = dislikeCount;
        CurrentUserReaction = currentUserReaction;
        IsAuthor = isAuthor;
    }

    public string CreatedAtFormatted => $"{CreatedAt:dd.MM.yyyy HH:mm}";
    public string AuthorLabel => IsAuthor ? " [author]" : string.Empty;
}