using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CityObjectItemViewModel
{
    public CityObjectDto Dto { get; }

    public CityObjectItemViewModel(CityObjectDto dto)
    {
        Dto = dto;
    }

    public long Id => Dto.Id;
    public string Name => Dto.Name;
    public string Description => Dto.Description;
    public string VoteDisplay => Dto.VoteDisplay;
}
