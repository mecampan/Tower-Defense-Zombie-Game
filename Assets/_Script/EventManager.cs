using System;

public static class EventManager
{
    private static string storedWarningMessage = string.Empty;
    public static Action<string> OnWarning;
    public static void UpdateWarningMessage(string message)
    {
        storedWarningMessage = message;
    }

    public static void TriggerWarning()
    {
        if (!string.IsNullOrEmpty(storedWarningMessage))
        {
            OnWarning?.Invoke(storedWarningMessage);
        }
    }

    // Tooltip System
    public static Action<string, bool> OnTooltip; // Pass tooltip message and visibility state
    public static void ShowTooltip(string message)
    {
        OnTooltip?.Invoke(message, true); // Trigger tooltip visibility ON
    }

    public static void HideTooltip()
    {
        OnTooltip?.Invoke(string.Empty, false); // Trigger tooltip visibility OFF
    }

    public static Action<int> OnRating;

    // Rating system for customers
    public static void UpdateRatingSystem(int ratingChange)
    {
        OnRating?.Invoke(ratingChange);
    }

}
