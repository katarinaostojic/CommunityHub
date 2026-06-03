using System;
using System.Collections.Generic;
using System.Text;
namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public class DonationCategory
{
    public long Id { get; private set; }
    public string Name { get; private set; }

    public DonationCategory(long id, string name)
    {
        Id = id;
        Name = name;
    }
}
