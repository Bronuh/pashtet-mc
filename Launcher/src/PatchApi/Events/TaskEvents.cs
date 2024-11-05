using KludgeBox.Events.EventTypes;
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