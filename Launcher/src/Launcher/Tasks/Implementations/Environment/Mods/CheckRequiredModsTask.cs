#region

using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Api;
using HashedFiles;
using KludgeBox.Events.Global;
using Launcher.Nodes;
using PatchApi.Events;

#endregion

namespace Launcher.Tasks.Environment.Mods;

public class CheckRequiredModsTask : LauncherTask
{
    private List<LauncherTask> _updateTasks = new();
    
    private List<RemoteFile> _modsToDownload = new();
    private List<string> _modsToRemove = new();
    public override string Name { get; } = "Проверка обязательных модов";

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

        var listReadyEvt = new ModsUpdateListsPreparedEvent(this, _modsToRemove, _modsToDownload);

        if (EventBus.PublishIsCancelled(listReadyEvt))
        {
            return;
        }
        
        _modsToRemove = listReadyEvt.ModsToRemove;
        _modsToDownload = listReadyEvt.ModsToDownload;
        
        if (!_modsToRemove.Any() && !_modsToDownload.Any())
            return;
        
        var sb = new StringBuilder();
        sb.AppendLine("Для игры на сервере необходимо обновить сборку модов.");
        sb.AppendLine("Ожидаются следующие изменения:");
        foreach (var modName in _modsToRemove)
        {
            sb.Append(" > ");
            sb.Append(modName);
            sb.Append(": ");
            if (_modsToDownload.Any(remote => remote.Name == modName))
            {
                sb.Append("Обновление");
            }
            else
            {
                sb.Append("Удаление");
            }

            sb.AppendLine();
        }

        foreach (var mod in _modsToDownload)
        {
            sb.Append(" > ");
            sb.Append(mod.Name);
            sb.Append(": Скачивание");
            sb.AppendLine();
        }
        
        bool updateAccepted = false;
        await Main.Popup.BeginBuild()
            .WithTitle("Доступно обновление сборки модов")
            .WithDescription(sb.ToString())
            .WithButton("Обновить", () => updateAccepted = true)
            .WithButton("Не обновлять")
            .WithCancelButton(() => Main.State.RunInterruptRequested = true)
            .PauseScheduler()
            .EnqueueAndWaitAsync();
        
        if(!updateAccepted)
            CancelUpdate();
    }

    private void CancelUpdate()
    {
        _modsToRemove.Clear();
        _modsToDownload.Clear();
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