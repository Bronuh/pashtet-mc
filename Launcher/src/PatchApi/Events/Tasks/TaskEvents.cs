using Common.Api;
using KludgeBox.Events.EventTypes;
using KludgeBox.Scheduling;
using Launcher.Tasks.Environment.Mods;
using Launcher.Tasks.Launch;

namespace PatchApi.Events;

public class ModsDeployTaskReadyEvent(DeployModpackTask task, string[] modsToDeploy) : CancellableEvent
{
    public DeployModpackTask Task { get; } = task;
    public string[] ModsToDeploy { get; set; } = modsToDeploy;
}

public class ModDeployingEvent(DeployModpackTask task, string sourcePath, string destinationPath) : CancellableEvent
{
    public DeployModpackTask Task { get; } = task;
    public string SourcePath { get; set; } = sourcePath;
    public string DestinationPath { get; set; } = destinationPath;
}

public class MinecraftStartingEvent : CancellableEvent
{
    public string MinecraftPath { get; set; }
    public string JavaPath { get; set; }
    public double MaxRam { get; set; }
    public string PlayerName { get; set; }
    public Scheduler Scheduler { get; set; }

    public MinecraftStartingEvent(string minecraftPath, string javaPath, double maxRam, string playerName, Scheduler scheduler)
    {
        MinecraftPath = minecraftPath;
        JavaPath = javaPath;
        MaxRam = maxRam;
        PlayerName = playerName;
        Scheduler = scheduler;
    }
}

public class ServersUpdatingEvent : CancellableEvent
{
    public string LocalServersFilePath { get; set; }
    public string RemoteServersFileDownloadUrl { get; set; }

    public ServersUpdatingEvent(string localServersFilePath, string remoteServersFileDownloadUrl)
    {
        LocalServersFilePath = localServersFilePath;
        RemoteServersFileDownloadUrl = remoteServersFileDownloadUrl;
    }
}

public class ModsUpdateListsPreparedEvent : CancellableEvent
{
    public CheckRequiredModsTask Task { get; }
    public List<string> ModsToRemove { get; set; } 
    public List<RemoteFile> ModsToDownload { get; set; }

    public ModsUpdateListsPreparedEvent(CheckRequiredModsTask task, List<string> modsToRemove, List<RemoteFile> modsToDownload)
    {
        Task = task;
        ModsToRemove = modsToRemove;
        ModsToDownload = modsToDownload;
    }
}

public class OptionalModsUpdateListsPreparedEvent : CancellableEvent
{
    public CheckOptionalModsTask Task { get; }
    public List<string> ModsToRemove { get; set; } 
    public List<RemoteFile> ModsToDownload { get; set; }

    public OptionalModsUpdateListsPreparedEvent(CheckOptionalModsTask task, List<string> modsToRemove, List<RemoteFile> modsToDownload)
    {
        Task = task;
        ModsToRemove = modsToRemove;
        ModsToDownload = modsToDownload;
    }
}


public enum CoreCheckType
{
    /// <summary>
    /// Check for JRE files integrity
    /// </summary>
    Java,
    
    /// <summary>
    /// Check for Minecraft files integrity
    /// </summary>
    Minecraft
}

public class CoreChecksPerformedEvent : CancellableEvent
{
    /// <summary>
    /// Indicates what kind of core files are checking now
    /// </summary>
    
    public CoreCheckType CheckType { get; }
    
    /// <summary>
    /// Result of existing core files check
    /// </summary>
    public bool CoreCheck { get; set; }
    
    /// <summary>
    /// Result of archived download file check
    /// </summary>
    public bool CoreDownloadCheck { get; set; }

    public CoreChecksPerformedEvent(CoreCheckType checkType, bool coreCheck, bool coreDownloadCheck)
    {
        CheckType = checkType;
        CoreCheck = coreCheck;
        CoreDownloadCheck = coreDownloadCheck;
    }
}