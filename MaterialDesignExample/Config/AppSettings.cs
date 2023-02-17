namespace SealWatch.Wpf.Config;

public class AppSettings
{
    public string AppVersion { get; set; } = "v.3.1.0";

    /// <summary>
    /// How long the SplashScreen will show.
    /// Maybe longer depending on how fast everything is loading.
    /// </summary>
    public int SplashScreenTime { get; set; } = 5;

    /// <summary>
    /// Determents the accuracy of decimal places in Analytic View
    /// </summary>
    public int Accuracy { get; set; } = 0;

    /// <summary>
    /// Text to show in AnalyticView for cutters that have no seal ordered the next seal
    /// </summary>
    public string NotOrderedText { get; set; } = "Nicht bestellt";

    /// <summary>
    /// Text to show in AnalyticView for cutters that have ordered the next seal
    /// </summary>
    public string OrderedText { get; set; } = "Bestellt";

    /// <summary>
    /// Determents the max width in columns on the location chart in the AnalyticView
    /// </summary>
    public int MaxGraphLocations { get; set; } = 7;

    /// <summary>
    /// Determents whether the program mocks with fake cutter and project data 
    /// </summary>
    public bool ShouldMock { get; set; } = true;
}