#region

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HashedFiles;
using Launcher.Nodes;

#endregion

namespace Launcher.Tasks.Implementations.Launch;

public class DeployModpackTask : LauncherTask
{
    public override double TotalWorkAmount => _workTotal;
    public override double CompletedWorkAmount => _workCompleted;

    private double _workTotal;
    private double _workCompleted;
    public override string Name { get; } = "Выгрузка модов в игру";
    
    protected override async Task Start()
    {
        var deployedMods = Directory.GetFiles(Paths.MinecraftModsDirPath.AsAbsolute());

        foreach (var mod in deployedMods)
        {
            Main.FileDeployer.UndeployFile(mod);
        }

        var modsToDeploy = BuildDeploymentListFrom(Paths.SnapshotModsDirPath.AsAbsolute())
            .Concat(BuildDeploymentListFrom(Paths.UserModsDirPath.AsAbsolute()))
            .ToArray();
        _workTotal = modsToDeploy.Length;
        
        foreach (var mod in modsToDeploy)
        {
            var name = Path.GetFileName(mod);
            var newPath = Path.Combine(Paths.MinecraftModsDirPath.AsAbsolute(), name);
            
            Main.FileDeployer.DeployFile(mod, newPath);
            _workCompleted++;
        }
    }

    private IEnumerable<string> BuildDeploymentListFrom(string path)
    {
        var modsToDeploy = Directory.GetFiles(path);
        return modsToDeploy;
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}