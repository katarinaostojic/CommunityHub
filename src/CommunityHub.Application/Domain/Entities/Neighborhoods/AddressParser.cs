using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityHub.Application.Domain.Entities.Neighborhoods;

public static class AddressParser
{
    public static bool TryParse(string fullAddress, out string streetName, out int streetNumber)
    {
        streetName = string.Empty;
        streetNumber = 0;
        string normalized = Normalize(fullAddress);
        int firstDigitIndex = FindFirstDigitIndex(normalized);
        if (firstDigitIndex == -1) return false;
        string streetPart = normalized[..firstDigitIndex].Trim().Trim(',', '.', '-', '/');
        string numberPart = new string(normalized[firstDigitIndex..].TakeWhile(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(streetPart)) return false;
        if (!int.TryParse(numberPart, out streetNumber)) return false;
        streetName = streetPart;
        return true;
    }

    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        string result = value.Trim().ToLowerInvariant();
        result = result.Replace("š", "s").Replace("đ", "d")
                       .Replace("č", "c").Replace("ć", "c").Replace("ž", "z");
        result = result.Replace("ulica", " ").Replace("ul.", " ").Replace("ul ", " ");
        var sb = new StringBuilder();
        foreach (char c in result)
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                sb.Append(c);
        result = sb.ToString();
        while (result.Contains("  "))
            result = result.Replace("  ", " ");
        return result.Trim();
    }

    private static int FindFirstDigitIndex(string value)
    {
        for (int i = 0; i < value.Length; i++)
            if (char.IsDigit(value[i]))
                return i;
        return -1;
    }
}
