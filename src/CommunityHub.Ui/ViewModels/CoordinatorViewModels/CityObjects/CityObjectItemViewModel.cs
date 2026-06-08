using CommunityHub.Application.DTOs.Neighborhoods.CityObjects;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class CityObjectItemViewModel : BaseViewModel
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

    private string _successMessage = string.Empty;
    public string SuccessMessage
    {
        get => _successMessage;
        set
        {
            SetProperty(ref _successMessage, value);
            OnPropertyChanged(nameof(HasSuccessMessage));
        }
    }
    public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);
}