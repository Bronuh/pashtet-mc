#region

using KludgeBox.Events.EventTypes;
using Launcher.Tasks;

#endregion

namespace PatchApi.Events;

public class TaskAboutToAddEvent(LauncherTask task) : CancellableEvent
{
    public LauncherTask Task { get; } = task;
}

public class TaskFinishedEvent(LauncherTask finishedTask, IEnumerable<LauncherTask> childTasks) : CancellableEvent
{
    public LauncherTask FinishedTask { get; } = finishedTask;
    public IEnumerable<LauncherTask> ChildTasks { get; set; } = childTasks;
}