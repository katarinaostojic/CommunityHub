using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Entities;

namespace CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

public interface ITrustRecordRepository
{
    TrustRecord GetByCitizen(long citizenId, long neighborhoodId);
    List<TrustRecord> GetByNeighborhood(long neighborhoodId);
    Dictionary<TrustLevel, int> GetTrustLevelCounts(long neighborhoodId);
}
