using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using IO;
using Launcher;

namespace Tasks.Implementations;

public enum ModAction
{
    /// <summary>
    /// No such mod found in the local storage
    /// </summary>
    Download,
    
    /// <summary>
    /// The mod is present in the local storage, but it's checksum does not natch the mod on the server
    /// </summary>
    Update,
    
    /// <summary>
    /// No such mod is present on the server
    /// </summary>
    Delete
}

public class ModUpdateTask : LauncherTask
{
    public string ModName { get; private set; }
    public ModAction Action { get; private set; }
    private string ModPath => Path.Combine(Paths.SnapshotModsDirPath.AsAbsolute(), ModName);

    private DownloadTask _downloader;
    
    public override double Progress => _downloader?.GetProgress() ?? 0;
    public override string Name { get; }
    
    public ModUpdateTask(string modName, ModAction action)
    {
        ModName = modName;
        Action = action;
        Name = $"{GetActionName()} {ModName}";
    }
    protected override async Task Start()
    {
        if(Action is ModAction.Delete)
        {
            DeleteMod();
        }

        if (Action is ModAction.Update)
        {
            DeleteMod();
            Action = ModAction.Download;
        }

        if (Action is ModAction.Download)
        {
            await DownloadMod();
        }
    }

    public override IEnumerable<LauncherTask> GetNextTasks()
    {
        return null;
    }

    private void DeleteMod()
    {
        File.Delete(ModPath);
    }

    private async Task DownloadMod()
    {
        var modUrl = Path.Combine(Urls.ModsDownloadBaseUrl, ModName);
        _downloader = new DownloadTask(modUrl, ModPath);
        await _downloader.RunAsync();
    }
    
    private string GetActionName()
    {
        return Action switch
        {
            ModAction.Delete => "Удаление",
            ModAction.Download => "Скачивание",
            ModAction.Update => "Обновление",
            _ => "ШТО"
        };
    }
}