using KludgeBox;
using KludgeBox.Events.Global;
using PatchApi;
using PatchApi.Events;

namespace LauncherPatches;

public class HelloWorldPatch : LauncherPatch
{
    public override void Run()
    {
        Log.Info($"Hello world!");
        EventBus.Subscribe<RunningMainTasksOnTaskManagerEvent>(OnMainTasksRun);
    }

    private static void OnMainTasksRun(RunningMainTasksOnTaskManagerEvent evt)
    {
        Log.Info($"Running main tasks hook");
    }
}