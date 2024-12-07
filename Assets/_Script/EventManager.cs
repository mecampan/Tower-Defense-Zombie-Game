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
}
