namespace CommunityHub.Application.Domain;

public enum AccessRequestResult
{
    Granted,
    RequestCreated,
    AlreadyPending,
    AlreadyMember
}