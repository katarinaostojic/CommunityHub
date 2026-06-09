using CommunityHub.Application.DTOs.Buildings;
using CommunityHub.Ui.Views.TenantViews;

namespace CommunityHub.Ui.Helpers.Tenant.Demo.Buildings;

public class BrowseBuildingsRequestDemoPlayer
{
    private const int MediumPause = 1400;
    private const int LongPause = 2600;
    private const int TypingPause = 450;

    private readonly TenantDemoRunner _demoRunner;
    private readonly BrowseBuildingsDemoContext _context;

    public BrowseBuildingsRequestDemoPlayer(
        TenantDemoRunner demoRunner,
        BrowseBuildingsDemoContext context)
    {
        _demoRunner = demoRunner;
        _context = context;
    }

    public async Task ShowAsync(CancellationToken token)
    {
        BuildingDto? building = _context.ViewModel.CurrentPageBuildings.FirstOrDefault();

        if (building == null)
            return;

        BuildingAccessRequestDialog? dialog = null;

        try
        {
            dialog = CreateDialog(building);
            dialog.Show();

            await _demoRunner.DelayAsync(token, MediumPause);

            await TypeApartmentNumberAsync(dialog, dialog.GetDemoUnitNumber(), token);

            await _demoRunner.DelayAsync(token, MediumPause);

            dialog.ClickSendRequestForDemo();
            _context.ShowRequestSentBanner(building);

            await _demoRunner.DelayAsync(token, LongPause);
        }
        finally
        {
            if (dialog?.IsVisible == true)
                dialog.Close();
        }
    }

    private BuildingAccessRequestDialog CreateDialog(BuildingDto building)
    {
        return new BuildingAccessRequestDialog(building, _context.User)
        {
            Owner = _context.GetOwner()
        };
    }

    private async Task TypeApartmentNumberAsync(
        BuildingAccessRequestDialog dialog,
        string unitNumber,
        CancellationToken token)
    {
        string typedText = string.Empty;
        dialog.SetUnitNumberForDemo(typedText);

        foreach (char character in unitNumber)
        {
            typedText += character;
            dialog.SetUnitNumberForDemo(typedText);

            await _demoRunner.DelayAsync(token, TypingPause);
        }
    }
}