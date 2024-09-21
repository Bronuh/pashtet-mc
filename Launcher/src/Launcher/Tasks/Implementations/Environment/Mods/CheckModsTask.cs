using System.Linq;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tasks.Implementations;

public class CheckModsTask : LauncherTask
{
    private List<LauncherTask> _updateTasks = new();
    
    private List<string> _modsToDownload = new();
    private List<string> _modsToUpdate = new();
    private List<string> _modsToRemove = new();
    public override string Name { get; } = "Проверка модов";
    protected override async Task Start()
    {
        var localMods = FilesystemContext.GetFiles(Paths.SnapshotModsDirPath.AsAbsolute(), false);
        var response = await HttpHelper.GetAsync("https://minecraft.bronuh.ru/api/mods/list");
        JArray jArray = JArray.Parse(response.Body);
        List<ModInfo> remoteMods = new();
        
        foreach (var jObject in jArray)
        {
            var mod = JsonConvert.DeserializeObject<ModInfo>(jObject.ToString());
            remoteMods.Add(mod);
        }
        
        // delete or update
        foreach (var localMod in localMods)
        {
            var remoteMod = remoteMods.FirstOrDefault(m => m.name == localMod.Name);

            if (remoteMod is null)
            {
                _modsToRemove.Add(localMod.Name);
            }
            else if (remoteMod.checksum != localMod.Checksum)
            {
                _modsToUpdate.Add(localMod.Name);
            }
        }
        
        // download
        foreach (var remoteMod in remoteMods)
        {
            var localMod = localMods.FirstOrDefault(m => m.Name == remoteMod.name);

            if (localMod is null)
            {
                _modsToDownload.Add(remoteMod.name);
            }
        }
    }

    record ModInfo(string name, string relativePath, string checksum);

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        if (_modsToRemove.Any() || _modsToUpdate.Any() || _modsToDownload.Any())
        {
            var backupTask = new BackupSnapshotTask();
            _updateTasks.Add(backupTask);
            _updateTasks.AddRange(_modsToRemove.Select(modName => new ModUpdateTask(modName, ModAction.Delete).AfterTasks(backupTask)));
            _updateTasks.AddRange(_modsToDownload.Select(modName => new ModUpdateTask(modName, ModAction.Download).AfterTasks(backupTask)));
            _updateTasks.AddRange(_modsToUpdate.Select(modName => new ModUpdateTask(modName, ModAction.Update).AfterTasks(backupTask)));
        }
        
        return _updateTasks;
    }
}