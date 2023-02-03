namespace SealWatch.Wpf.Config;

public class AppSettings
{
    /// <summary>
    /// How long the SplashScreen will show.
    /// Maybe longer depending on how fast everything is loading.
    /// </summary>
    public int SplashScreenTime { get; set; } = 5;

    /// <summary>
    /// Determents the accuracy of decimal places in Analytic View
    /// </summary>
    public int Accuracy { get; set; } = 1;

    public string AppVersion { get; set; } = "v.2.0";
}