#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;

#endregion

namespace Launcher.Tasks.Launch;

public class DeleteServerResourcepackTask : LauncherTask
{
    public override string Name { get; } = "Удаление старого серверного ресурспака";
    protected override async Task Start()
    {
        var resourcePacksDir = Path.Combine(Paths.MinecraftDirPath.AsAbsolute(), "server-resource-packs");
        var packs = Directory.GetFiles(resourcePacksDir);
        
        foreach (var pack in packs)
        {
            File.Delete(pack);
        }
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}