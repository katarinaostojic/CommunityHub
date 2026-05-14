using System;
using System.Collections.Generic;
using System.Text;

using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Application.Domain.Neighborhoods.NeighborhoodRepositoryInterfaces;

public interface IEventRepository
{
    long Create(Event ev);
    List<Event> GetByNeighborhood(long neighborhoodId);
    Event? GetById(long eventId);
    void Update(Event ev);
    void AddRegistration(long eventId, long citizenId, List<long> itemIds);
}
