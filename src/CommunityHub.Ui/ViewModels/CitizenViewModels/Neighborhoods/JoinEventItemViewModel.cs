using System;
using System.Collections.Generic;
using System.Text;
using CommunityHub.Application.Domain.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class JoinEventItemViewModel
{
    private readonly EventItem _item;

    public JoinEventItemViewModel(EventItem item)
    {
        _item = item;
    }

    public long ItemId => _item.Id;
    public string ItemName => _item.Name;
    public bool IsAvailable => !_item.IsTaken;
    public string StatusDisplay => _item.IsTaken ? "Already taken" : "Available";
    public string StatusColor => _item.IsTaken ? "Red" : "Green";
}
