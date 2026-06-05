using CommunityHub.Application.Domain.Entities.Shared;
using System;
using System.Linq;
using System.Windows;

namespace CommunityHub.Ui.Helpers;

public static class ThemeManager
{
    public static void ApplyTheme(UserRole role)
    {
        string themePath = role switch
        {
            UserRole.Tenant => "Themes/TenantTheme.xaml",
            UserRole.Coordinator => "Themes/CoordinatorTheme.xaml",

            // OVDE UBACITE KAD ODRADITE
            UserRole.Manager => "Themes/TenantTheme.xaml",
            UserRole.Citizen => "Themes/CitizenTheme.xaml",

            _ => "Themes/TenantTheme.xaml"
        };

        RemoveCurrentTheme();

        ResourceDictionary newTheme = new ResourceDictionary
        {
            Source = new Uri(themePath, UriKind.Relative)
        };

        System.Windows.Application.Current.Resources.MergedDictionaries.Add(newTheme);
    }

    private static void RemoveCurrentTheme()
    {
        ResourceDictionary? existingTheme = System.Windows.Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(dictionary =>
                dictionary.Source != null &&
                dictionary.Source.OriginalString.Contains("Themes/"));

        if (existingTheme != null)
        {
            System.Windows.Application.Current.Resources.MergedDictionaries.Remove(existingTheme);
        }
    }
}