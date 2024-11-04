#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;

#endregion

namespace Launcher.Tasks.Environment.Mods;


public class ModRemoveTask : LauncherTask
{
    public string ModName { get; private set; }
    private string ModPath => Path.Combine(Paths.SnapshotModsDirPath.AsAbsolute(), ModName);

    
    public override string Name { get; }
    
    public ModRemoveTask(string modName)
    {
        ModName = modName;
        Name = $"{GetActionName()} {ModName}";
    }
    protected override async Task Start()
    {
            DeleteMod();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }

    private void DeleteMod()
    {
        File.Delete(ModPath);
    }

    
    
    private string GetActionName()
    {
        return "Удаление";
    }
}