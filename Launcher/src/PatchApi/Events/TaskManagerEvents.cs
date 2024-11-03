using KludgeBox.Events.EventTypes;
using Launcher.Tasks;

namespace PatchApi.Events;

public class TaskAboutToAddEvent(LauncherTask Task) : CancellableEvent;
public class TaskFinishedEvent(LauncherTask FinishedTask, ref IEnumerable<LauncherTask> ChildTasks) : CancellableEvent;