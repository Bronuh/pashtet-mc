using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using Common.Api;
using IO;
using Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tasks.Implementations;

public class CheckModsTask : LauncherTask
{
    private List<LauncherTask> _updateTasks = new();
    
    private List<RemoteFile> _modsToDownload = new();
    private List<string> _modsToRemove = new();
    public override string Name { get; } = "Проверка модов";

    protected override async Task Start()
    {
        var checksumProvider = Main.ChecksumProvider;
        var localMods = Directory.GetFiles(Paths.SnapshotModsDirPath.AsAbsolute())
            .Select(path => new LocalFile(path, checksumProvider)).ToList();

        var remoteMods = await Main.ApiProvider.GetRequiredModsListAsync();
        
        // delete or update
        foreach (var localMod in localMods)
        {
            var remoteMod = remoteMods.Files.FirstOrDefault(m => m.Name == localMod.FileName);
            
            if (remoteMod is null)
            {
                _modsToRemove.Add(localMod.FileName);
            }
            else if (remoteMod.Checksum != localMod.GetPrecalculatedChecksum())
            {
                _modsToRemove.Add(localMod.FileName);
                _modsToDownload.Add(remoteMod);
            }
        }
        
        // download
        foreach (var remoteMod in remoteMods.Files)
        {
            var localMod = localMods.FirstOrDefault(m => m.FileName == remoteMod.Name);

            if (localMod is null)
            {
                _modsToDownload.Add(remoteMod);
            }
        }
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        if (_modsToRemove.Any() || _modsToDownload.Any())
        {
            //var backupTask = new BackupSnapshotTask();
            //_updateTasks.Add(backupTask);
            var removeTasks = _modsToRemove.Select(modName => new ModRemoveTask(modName)).ToArray();
            _updateTasks.AddRange(removeTasks);
            _updateTasks.AddRange(_modsToDownload.Select(mod => new DownloadModTask(mod).AfterTasks(removeTasks)));
        }
        
        return _updateTasks;
    }
}