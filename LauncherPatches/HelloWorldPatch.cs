using KludgeBox;
using PatchApi;

namespace LauncherPatches;

public class HelloWorldPatch : LauncherPatch
{
    public override void Run()
    {
        Log.Info($"Hello world!");
    }
}