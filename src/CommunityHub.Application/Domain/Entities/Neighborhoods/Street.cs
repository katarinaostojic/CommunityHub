using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class Street
{
    public long Id { get; private set; }
    public long NeighborhoodId { get; private set; }
    public string StreetName { get; private set; }
    public int StartNumber { get; private set; }
    public int EndNumber { get; private set; }

    public Street(long id, long neighborhoodId, string streetName, int startNumber, int endNumber)
    {
        Id = id;
        NeighborhoodId = neighborhoodId;
        StreetName = streetName;
        StartNumber = startNumber;
        EndNumber = endNumber;
    }

    public override string ToString()
    {
        return $"{StreetName} ({StartNumber}-{EndNumber})";
    }
}
