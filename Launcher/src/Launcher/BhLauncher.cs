using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using KludgeBox.Scheduling;

namespace Launcher;

public class BhLauncher
{
    public string MinecraftPath { get; private set; }
    public string JrePath { get; private set; }
    public string PlayerName { get; private set; }
    public double Ram { get; private set; }

    private Scheduler _scheduler;

    private int GbToMbFactor => 1024;
    
    public BhLauncher(string minecraftPath, string jrePath, Scheduler scheduler, string playerName, double ram)
    {
        MinecraftPath = minecraftPath;
        JrePath = jrePath;
        _scheduler = scheduler;
        PlayerName = playerName;
        Ram = ram;
    }

    public async Task Run()
    {
        //var coroutine = await PrepareMinecraftLaunchCoroutine();
        await RunMinecraftAsync();
        //var task = _scheduler.RunCoroutine(coroutine);
    }

    private int GbToMb(double gb)
    {
        return Mathf.RoundToInt(gb * GbToMbFactor);
    }

    private async Task<int> RunMinecraftAsync()
    {
        var launcher = await PrepareLauncher();
        var wrapper = await BuildProcess(launcher);
        
        wrapper.OutputReceived += (sender, msg) =>
        {
            if (Log4JProcessor.IsLog4JOutput(msg))
            {
                Log4JProcessor.ProcessLog4JOutput(msg);
            }
            else
            {
                Log.Info(msg);
            }
        };
        wrapper.StartWithEvents();
        
        var exitCode = await wrapper.WaitForExitTaskAsync();
        
        Log.Info($"Minecraft process exited with code {wrapper.Process.ExitCode}");
        
        return exitCode;
    }
    
    private async Task<IEnumerator<ITiming>> PrepareMinecraftLaunchCoroutine()
    {
        var launcher = await PrepareLauncher();
        var wrapper = await BuildProcess(launcher);
        var coroutine = MinecraftLaunchCoroutine(wrapper);

        wrapper.Process.OutputDataReceived += (sender, msg) =>
        {
            Log.Info(msg.Data);
        };

        wrapper.Process.ErrorDataReceived += (sender, msg) =>
        {
            Log.Error(msg.Data);
        };
        
        /*wrapper.OutputReceived += (sender, msg) =>
        {
            Log.Info(msg);
        };*/
        
        return coroutine;
    }

    private IEnumerator<ITiming> MinecraftLaunchCoroutine(ProcessWrapper wrapper)
    {
        wrapper.StartWithEvents();
        while (!wrapper.Process.HasExited)
        {
            yield return Wait.Ticks(10);
        }
        Log.Info($"Minecraft process exited with code {wrapper.Process.ExitCode}");
    }

    private async Task<MinecraftLauncher> PrepareLauncher()
    {
        var path = new MinecraftPath(MinecraftPath);
        path.Runtime = JrePath;
		
        var launcher = new MinecraftLauncher(path);
		
        // add event handlers
        launcher.FileProgressChanged += (sender, args) =>
        {
            Log.Debug($"Name: {args.Name}");
            Log.Debug($"Type: {args.EventType}");
            Log.Debug($"Total: {args.TotalTasks}");
            Log.Debug($"Progressed: {args.ProgressedTasks}");
        };
        launcher.ByteProgressChanged += (sender, args) =>
        {
            Log.Debug($"{args.ProgressedBytes} bytes / {args.TotalBytes} bytes");
        };

        return launcher;
    }

    private async Task<ProcessWrapper> BuildProcess(MinecraftLauncher launcher)
    {
        var ramMb = GbToMb(Ram);
        var process = await launcher.BuildProcessAsync("Forge 1.20.1", new MLaunchOption
        {
            Session = MSession.CreateOfflineSession(PlayerName),
            MaximumRamMb = ramMb,
            JavaPath = JrePath
        });

        var wrapper = new ProcessWrapper(process);
        
        
        return wrapper;
    }
}