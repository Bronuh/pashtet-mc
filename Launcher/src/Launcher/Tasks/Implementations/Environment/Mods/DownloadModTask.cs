#region

using System.IO;
using System.Threading.Tasks;
using Common.Api;
using Common.IO.Http;
using HashedFiles;

#endregion

namespace Launcher.Tasks.Implementations.Environment.Mods;

public class DownloadModTask : LauncherTask
{
    public override string Name { get; }
    public string ModName { get; }
    private DownloadTask _downloader;
    private RemoteFile _file;
    public override double Progress => _downloader?.GetProgress() ?? 0;
    private string ModPath => Path.Combine(Paths.SnapshotModsDirPath.AsAbsolute(), ModName);

    public DownloadModTask(RemoteFile file)
    {
        _file = file;
        ModName = file.Name;
        Name = $"Скачивание {file.Name}";
    }
    protected override async Task Start()
    {
        await DownloadMod();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
    
    private async Task DownloadMod()
    {
        _downloader = HttpHelper.PrepareDownload(_file.Url, ModPath);
        await _downloader.RunAsync();
    }

}