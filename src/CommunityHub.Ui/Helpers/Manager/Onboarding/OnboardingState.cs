using System.IO;

namespace CommunityHub.Ui.Helpers.Manager.Onboarding;

public static class OnboardingState
{
    private static readonly string FlagFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "CommunityHub",
        "manager_wizard_seen.flag");

    public static bool HasSeenWizard()
    {
        return File.Exists(FlagFilePath);
    }

    public static void MarkWizardAsSeen()
    {
        string? directory = Path.GetDirectoryName(FlagFilePath);
        if (directory != null)
            Directory.CreateDirectory(directory);

        File.WriteAllText(FlagFilePath, DateTime.Now.ToString("O"));
    }


    // pri sledecem pokretanju aplikacije (kao da je to prvi put).
    public static void ResetWizard()
    {
        if (File.Exists(FlagFilePath))
            File.Delete(FlagFilePath);
    }
}