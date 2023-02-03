namespace SealWatch.Wpf.Config;

public class AppSettings
{
    /// <summary>
    /// How long the SplashScreen will show.
    /// Maybe longer depending on how fast everything is loading.
    /// </summary>
    public int SplashScreenTime { get; set; } = 5;

    public string AppVersion { get; set; } = "v.2.0";
}