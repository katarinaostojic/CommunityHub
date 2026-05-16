using CommunityHub.Application.DTOs.Neighborhoods;

namespace CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;

public class JoinEventItemViewModel
{
    private readonly EventItemDto _item;

    public JoinEventItemViewModel(EventItemDto item)
    {
        _item = item;
    }

    public long ItemId => _item.Id;
    public string ItemName => _item.Name;
    public bool IsAvailable => _item.IsAvailable;
    public string StatusDisplay => _item.StatusDisplay;
    public string StatusColor => _item.IsTaken ? "Red" : "Green";
}