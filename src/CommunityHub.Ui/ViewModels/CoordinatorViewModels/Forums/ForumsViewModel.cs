using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Application.Services.Entities.Neighborhoods.Forums;
using System.Collections.ObjectModel;

namespace CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;

public class ForumsViewModel : BaseViewModel
{
    private readonly ForumService _forumService;
    private readonly long _coordinatorId;

    private ObservableCollection<ForumItemViewModel> _forums = new();
    private string _searchText = string.Empty;
    private List<ForumItemViewModel> _allForums = new();

    public ForumsViewModel(ForumService forumService, long coordinatorId)
    {
        _forumService = forumService;
        _coordinatorId = coordinatorId;
        LoadForums();
    }

    public ObservableCollection<ForumItemViewModel> Forums
    {
        get => _forums;
        private set => SetProperty(ref _forums, value);
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            SetProperty(ref _searchText, value);
            ApplySearch();
        }
    }

    public void LoadForums()
    {
        _allForums = _forumService.GetAll(_coordinatorId)
            .Select(f => new ForumItemViewModel(f))
            .ToList();
        Forums = new ObservableCollection<ForumItemViewModel>(_allForums);
    }

    private void ApplySearch()
    {
        if (string.IsNullOrWhiteSpace(_searchText))
        {
            Forums = new ObservableCollection<ForumItemViewModel>(_allForums);
            return;
        }

        string lowered = _searchText.ToLower();
        var filtered = _allForums
            .Where(f => f.Title.ToLower().Contains(lowered) || f.Forum.Description.ToLower().Contains(lowered))
            .ToList();
        Forums = new ObservableCollection<ForumItemViewModel>(filtered);
    }
}