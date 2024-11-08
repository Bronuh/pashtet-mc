using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Api;
using Common.IO.Checksum;
using HashedFiles;
using KludgeBox.Events.Global;
using Launcher.Nodes;
using Newtonsoft.Json;
using PatchApi.Events;

namespace Launcher.Tasks.Environment.Mods;

public class CheckOptionalModsTask : LauncherTask
{
    private List<LauncherTask> _updateTasks = new();
    
    private List<RemoteFile> _modsToDownload = new();
    private List<string> _modsToRemove = new();
    public override string Name { get; } = "Проверка обязательных модов";
    protected override async Task Start()
    {
        IChecksumProvider checksumProvider = Main.ChecksumProvider;
        List<LocalFile> localMods = Directory.GetFiles(Paths.SnapshotOptionalModsDirPath.AsAbsolute())
            .Select(path => new LocalFile(path, checksumProvider)).ToList();
        List<string> enabledMods = GetEnabledModsList();

        var remoteMods = await Main.ApiProvider.GetOptionalModsListAsync();
        var remoteModsInfo = await Main.ApiProvider.GetOptionalModsInfoAsync();
        
        // delete or update
        foreach (var localMod in localMods)
        {
            var remoteMod = remoteMods.Files.FirstOrDefault(m => m.Name == localMod.FileName);
            var enabledMod = enabledMods.FirstOrDefault(m => m == localMod.FileName);
            
            if (remoteMod is null || enabledMod is null)
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
        foreach (var enabledMod in enabledMods)
        {
            var localMod = localMods.FirstOrDefault(m => m.FileName == enabledMod);
            var remoteMod = remoteMods.Files.FirstOrDefault(m => m.Name == enabledMod);

            if (localMod is null && remoteMod is not null)
            {
                _modsToDownload.Add(remoteMod);
            }
        }
        
        var listReadyEvt = new OptionalModsUpdateListsPreparedEvent(this, _modsToRemove, _modsToDownload);

        if (EventBus.PublishIsCancelled(listReadyEvt))
        {
            return;
        }
        
        _modsToRemove = listReadyEvt.ModsToRemove;
        _modsToDownload = listReadyEvt.ModsToDownload;
    }

    private List<string> GetEnabledModsList()
    {
        List<string> enabledMods;
        var enabledModsSettingsValue = Main.Settings.GetCustom("EnabledMods");
        if (!String.IsNullOrWhiteSpace(enabledModsSettingsValue))
        {
            try
            {
                enabledMods = JsonConvert.DeserializeObject<List<string>>(enabledModsSettingsValue);
            }
            catch (Exception e)
            {
                Log.Error($"Не удалось прочитать список активных модов: {e.Message}");
            }
        }

        enabledMods = new();

        return enabledMods;
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        if (_modsToRemove.Any() || _modsToDownload.Any())
        {
            //var backupTask = new BackupSnapshotTask();
            //_updateTasks.Add(backupTask);
            var removeTasks = _modsToRemove.Select(modName => new ModRemoveTask(modName, true)).ToArray();
            _updateTasks.AddRange(removeTasks);
            _updateTasks.AddRange(_modsToDownload.Select(mod => new DownloadModTask(mod, true).AfterTasks(removeTasks)));
        }
        
        return _updateTasks;
    }
}