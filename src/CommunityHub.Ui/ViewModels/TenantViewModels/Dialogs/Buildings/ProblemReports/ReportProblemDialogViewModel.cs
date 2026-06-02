using CommunityHub.Application.Domain.Entities.Buildings.ProblemReports;
using CommunityHub.Ui.Extensions;

namespace CommunityHub.Ui.ViewModels.TenantViewModels.Buildings.ProblemReports;

public class ReportProblemDialogViewModel : BaseViewModel
{
    private string _descriptionError = string.Empty;
    private bool _hasDescriptionError;
    private string _priorityError = string.Empty;
    private bool _hasPriorityError;

    public ReportProblemDialogViewModel(string buildingInfo)
    {
        BuildingInfo = buildingInfo;
        PriorityOptions = Enum.GetValues<ProblemPriority>()
            .Select(p => p.ToDisplayString())
            .ToList();
    }

    public string BuildingInfo { get; }
    public List<string> PriorityOptions { get; }

    public string DescriptionError
    {
        get => _descriptionError;
        private set => SetProperty(ref _descriptionError, value);
    }

    public bool HasDescriptionError
    {
        get => _hasDescriptionError;
        private set => SetProperty(ref _hasDescriptionError, value);
    }

    public string PriorityError
    {
        get => _priorityError;
        private set => SetProperty(ref _priorityError, value);
    }

    public bool HasPriorityError
    {
        get => _hasPriorityError;
        private set => SetProperty(ref _hasPriorityError, value);
    }

    public bool Validate(string description, int priorityIndex)
    {
        bool isDescriptionValid = ValidateDescription(description);
        bool isPriorityValid = ValidatePriority(priorityIndex);

        return isDescriptionValid && isPriorityValid;
    }

    public ProblemPriority GetPriority(int priorityIndex)
    {
        return (ProblemPriority)priorityIndex;
    }

    private bool ValidateDescription(string description)
    {
        string? error = ProblemReport.ValidateDescription(description);

        HasDescriptionError = error != null;
        DescriptionError = error ?? string.Empty;

        return error == null;
    }

    private bool ValidatePriority(int priorityIndex)
    {
        bool isValid = priorityIndex >= 0 && priorityIndex < PriorityOptions.Count;

        HasPriorityError = !isValid;
        PriorityError = isValid ? string.Empty : "Please select problem priority.";

        return isValid;
    }
}