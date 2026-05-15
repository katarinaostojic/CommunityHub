namespace CommunityHub.Application.DTOs.Neighborhoods;

using CommunityHub.Application.Domain;
using CommunityHub.Application.Domain.Neighborhoods;

public class NeighborhoodDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string CityName { get; init; }
    public string CountryName { get; init; }
    public long CoordinatorId { get; init; }
    public List<string> ImagePaths { get; init; }

    public NeighborhoodDto(long id, string name, string description,
        string cityName, string countryName, long coordinatorId, List<string> imagePaths)
    {
        Id = id;
        Name = name;
        Description = description;
        CityName = cityName;
        CountryName = countryName;
        CoordinatorId = coordinatorId;
        ImagePaths = imagePaths;
    }

    public string Location => $"{CityName}, {CountryName}";
}

public class NeighborhoodMembershipDto
{
    public long Id { get; init; }
    public string CitizenName { get; init; }
    public string CitizenSurname { get; init; }
    public string? CitizenAddress { get; init; }
    public DateTime JoinedAt { get; init; }

    public NeighborhoodMembershipDto(long id, string citizenName, string citizenSurname,
        string? citizenAddress, DateTime joinedAt)
    {
        Id = id;
        CitizenName = citizenName;
        CitizenSurname = citizenSurname;
        CitizenAddress = citizenAddress;
        JoinedAt = joinedAt;
    }

    public string FullName => $"{CitizenName} {CitizenSurname}";
    public string Address => CitizenAddress ?? "No address";
    public string JoinedAtFormatted => $"Joined: {JoinedAt:dd.MM.yyyy}";
}

public class NeighborhoodAccessRequestDto
{
    public long Id { get; init; }
    public string CitizenName { get; init; }
    public string CitizenSurname { get; init; }
    public string? CitizenAddress { get; init; }
    public string NeighborhoodName { get; init; }
    public DateTime CreatedAt { get; init; }
    public RequestStatus Status { get; init; }
    public string? RejectionReason { get; init; }

    public NeighborhoodAccessRequestDto(
        long id,
        string citizenName,
        string citizenSurname,
        string? citizenAddress,
        string neighborhoodName,
        DateTime createdAt,
        RequestStatus status,
        string? rejectionReason = null)
    {
        Id = id;
        CitizenName = citizenName;
        CitizenSurname = citizenSurname;
        CitizenAddress = citizenAddress;
        NeighborhoodName = neighborhoodName;
        CreatedAt = createdAt;
        Status = status;
        RejectionReason = rejectionReason;
    }

    public string CitizenFullName => $"{CitizenName} {CitizenSurname}";
    public string Address => CitizenAddress ?? "No address";
    public bool HasRejectionReason => Status == RequestStatus.Rejected && RejectionReason != null;
}

public class ForumDto
{
    public long Id { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public long CoordinatorId { get; init; }
    public string CoordinatorFullName { get; init; }
    public bool IsClosed { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsAuthor { get; init; }

    public ForumDto(long id, string title, string description, long coordinatorId,
        string coordinatorFullName, bool isClosed, DateTime createdAt, bool isAuthor)
    {
        Id = id;
        Title = title;
        Description = description;
        CoordinatorId = coordinatorId;
        CoordinatorFullName = coordinatorFullName;
        IsClosed = isClosed;
        CreatedAt = createdAt;
        IsAuthor = isAuthor;
    }

    public string CreatedAtFormatted => $"{CreatedAt:dd.MM.yyyy HH:mm}";
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