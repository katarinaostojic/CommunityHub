using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings;
using CommunityHub.Application.Domain.RepositoryInterfaces.Buildings.CommonRooms;
using CommunityHub.Application.DTOs.Buildings.CommonRooms;
using CommunityHub.Application.Mappings.Buildings.CommonRooms;

namespace CommunityHub.Application.Services.Entities.Buildings.CommonRooms;

public class CommonRoomRequestService
{
    private readonly ICommonRoomRequestRepository _requestRepository;
    private readonly IBuildingMembershipRepository _membershipRepository;
    private readonly CommonRoomRequestApprovalService _approvalService;
    private readonly CommonRoomRequestCommandService _commandService;

    public CommonRoomRequestService(
        ICommonRoomRequestRepository requestRepository,
        IBuildingMembershipRepository membershipRepository,
        CommonRoomRequestApprovalService approvalService,
        CommonRoomRequestCommandService commandService)
    {
        _requestRepository = requestRepository;
        _membershipRepository = membershipRepository;
        _approvalService = approvalService;
        _commandService = commandService;
    }

    public List<CommonRoomRequestDto> GetRequestsByCommonRoom(long commonRoomId)
    {
        return _requestRepository.GetAllByCommonRoom(commonRoomId).ToDtoList();
    }

    public CommonRoomRequestDto? GetById(long requestId)
    {
        return _requestRepository.GetById(requestId)?.ToDto();
    }

    public List<DateTime> GetFreeDaysInRange(long requestId)
    {
        return _approvalService.GetFreeDaysInRange(requestId);
    }

    public List<(DateTime, DateTime)> FindAlternativeRanges(long requestId)
    {
        return _approvalService.FindAlternativeRanges(requestId);
    }

    public void ApproveWithDate(long requestId, DateTime selectedDate)
    {
        _approvalService.ApproveWithDate(requestId, selectedDate);
    }

    public void RejectRequest(long requestId)
    {
        _commandService.RejectRequest(requestId);
    }

    public void ProposeAlternative(long requestId, int alternativeIndex)
    {
        _approvalService.ProposeAlternative(requestId, alternativeIndex);
    }

    public List<CommonRoomRequestDto> GetByTenantAndBuilding(long tenantId, long buildingId)
    {
        EnsureTenantHasBuildingMembership(tenantId, buildingId);
        return _requestRepository.GetByTenantAndBuilding(tenantId, buildingId).ToDtoList();
    }

    public void CreateRequest(long commonRoomId, long tenantId, DateTime dateFrom, DateTime dateTo)
    {
        _commandService.CreateRequest(commonRoomId, tenantId, dateFrom, dateTo);
    }

    public void CancelRequest(long requestId)
    {
        _commandService.CancelRequest(requestId);
    }

    public void AcceptProposedDateChange(long requestId)
    {
        _commandService.AcceptProposedDateChange(requestId);
    }

    private void EnsureTenantHasBuildingMembership(long tenantId, long buildingId)
    {
        if (!_membershipRepository.Exists(tenantId, buildingId))
            throw new InvalidOperationException("Tenant is not a member of this building.");
    }
}