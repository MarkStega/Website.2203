using Material.Blazor;

namespace Website.Client;

public static class Utilities
{
    public const string NonInteraction = "non_interaction";
    public const string EventLabel = "event_label";
    public const string EventCategory = "event_category";
    public const string DialogActions = "Dialog Actions";

    public static MBLoggingServiceConfiguration GetDefaultLoggingServiceConfiguration() => new()
    {
        LoggingLevel = MBLoggingLevel.Warning
    };


    public static MBToastServiceConfiguration GetDefaultToastServiceConfiguration() => new()
    {
        Position = MBToastPosition.TopRight,
        CloseMethod = MBNotifierCloseMethod.Timeout,
    };


    public static MBSnackbarServiceConfiguration GetDefaultSnackbarServiceConfiguration() => new()
    {
        CloseMethod = MBNotifierCloseMethod.TimeoutAndDismissButton,
        Leading = false,
        Stacked = false,
        Timeout = 5000
    };
}
