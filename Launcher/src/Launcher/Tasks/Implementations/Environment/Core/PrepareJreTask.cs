#region

using System.IO;
using System.Threading.Tasks;
using HashedFiles;
using KludgeBox.Events.Global;
using Launcher.Nodes;
using PatchApi.Events;

#endregion

namespace Launcher.Tasks.Environment.Core;

public class PrepareJreTask : LauncherTask
{
    private LauncherTask _nextTask;
    public override string Name { get; } = "Подготовка Java";
    
    protected override async Task Start()
    {
        var fileCheck = File.Exists(Paths.JreExecutablePath.AsAbsolute());
        var downloadCheck = File.Exists(Paths.JreFileName.AsAbsolute());

        var evt = new CoreChecksPerformedEvent(CoreCheckType.Java, fileCheck, downloadCheck);
        
        if (EventBus.PublishIsCancelled(evt))
            return;

        fileCheck = evt.CoreCheck;
        downloadCheck = evt.CoreDownloadCheck;
        
        if (fileCheck)
        {
            Main.State.IsJavaReady = true;
            return;
        }

        if (downloadCheck)
        {
            _nextTask = new UnpackJreTask();
            return;
        }

        bool downloadAccepted = false;
        await Main.Popup.BeginBuild()
            .WithTitle("Требуется скачать Java")
            .WithDescription("Для запуска Minecraft необходимо скачать и установить Java.")
            .WithButton("Скачать", () => downloadAccepted = true)
            .WithButton("Не скачивать")
            .PauseScheduler()
            .EnqueueAndWaitAsync();
        
        if (downloadAccepted)
            _nextTask = new DownloadJreTask();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return [_nextTask];
    }
}