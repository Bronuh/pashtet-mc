namespace Launcher;

public class LauncherState
{
    public bool IsMinecraftReady { get; set; }
    public bool IsJavaReady { get; set; }
    public bool IsMinecraftRunning { get; set; }
    public bool RunInterruptRequested { get; set; }
    
    public bool CanLaunch => (IsMinecraftReady & IsJavaReady) && !IsMinecraftRunning;
}