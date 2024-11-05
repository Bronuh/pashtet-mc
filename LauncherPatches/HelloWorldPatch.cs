#region

using KludgeBox;
using KludgeBox.Events;
using KludgeBox.Events.Global;
using PatchApi;
using PatchApi.Events;

#endregion

namespace LauncherPatches;

public class HelloWorldPatch : LauncherPatch
{
    protected override void Run()
    {
        Log.Info($"Hello world!");
        Log.Debug("THis is debug patch");
        EventBus.Subscribe<RunningMainTasksOnTaskManagerEvent>(OnMainTasksRun);
    }

    private static void OnMainTasksRun(RunningMainTasksOnTaskManagerEvent evt)
    {
        Log.Info($"Running main tasks hook");
    }

    [EventListener]
    public static void AutoRegisteringListener(RunningMainTasksOnTaskManagerEvent evt)
    {
        Log.Info($"Running main tasks hook AUTOMATICALLY");
    }
}