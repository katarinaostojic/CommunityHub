using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain;

namespace CommunityHub.Application.Domain.RepositoryInterfaces.Neighborhoods;

public interface ITrustRecordRepository
{
    TrustRecord GetByCitizen(long citizenId, long neighborhoodId);
    List<TrustRecord> GetByNeighborhood(long neighborhoodId);
    Dictionary<TrustLevel, int> GetTrustLevelCounts(long neighborhoodId);
}
