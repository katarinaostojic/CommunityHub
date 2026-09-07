using System.Collections.Generic;
using CommunityHub.Application.Domain.Entities.Shared;

namespace CommunityHub.Ui.Helpers.Citizen;

public static class NavigationHistory
{
    private static readonly Stack<NavigationEntry> _backStack = new();
    private static readonly Stack<NavigationEntry> _forwardStack = new();

    public static bool CanGoBack => _backStack.Count > 0;
    public static bool CanGoForward => _forwardStack.Count > 0;

    public static void NavigateTo(NavigationEntry current)
    {
        _backStack.Push(current);
        _forwardStack.Clear();
    }

    public static NavigationEntry? GoBack(NavigationEntry current)
    {
        if (_backStack.Count == 0) return null;
        _forwardStack.Push(current);
        return _backStack.Pop();
    }

    public static NavigationEntry? GoForward(NavigationEntry current)
    {
        if (_forwardStack.Count == 0) return null;
        _backStack.Push(current);
        return _forwardStack.Pop();
    }

    public static void Clear()
    {
        _backStack.Clear();
        _forwardStack.Clear();
    }
}

public record NavigationEntry(
    string PageKey,
    User User,
    long? NeighborhoodId = null,
    string? NeighborhoodName = null);
