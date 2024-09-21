namespace Launcher;

public static class ServerInfo
{
    public static bool IsReady = false;
    
    public static bool IsOnline;
    public static int CurrentPlayers;
    public static int MaximumPlayers;
    public static string Version;
    public static long Latency;
    public static IEnumerable<string> PlayerList;
}