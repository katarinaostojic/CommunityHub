using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain;

public class Neighborhood
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public long CityId { get; private set; }
    public string CityName { get; private set; }
    public string CountryName { get; private set; }
    public decimal Budget { get; private set; }
    public long CoordinatorId { get; private set; }
    public List<Street> Streets { get; private set; }

    public Neighborhood(long id, string name, string description, long cityId, string cityName, string countryName, decimal budget, long coordinatorId)
    {
        Id = id;
        Name = name;
        Description = description;
        CityId = cityId;
        CityName = cityName;
        CountryName = countryName;
        Budget = budget;
        CoordinatorId = coordinatorId;
        Streets = new List<Street>();
    }

    public void AddStreet(Street street)
    {
        Streets.Add(street);
    }
}