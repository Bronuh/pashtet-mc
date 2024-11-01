using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;

namespace Tasks.Implementations;


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