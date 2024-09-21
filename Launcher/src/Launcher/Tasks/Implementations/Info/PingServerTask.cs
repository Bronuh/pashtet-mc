using System.Threading.Tasks;
using Launcher;
using MineStatLib;

namespace Tasks.Implementations.Info;

public class PingServerTask : LauncherTask
{
    public override string Name { get; } = "Проверка состояния сервера";
    private MineStat _mineStat;
    private double _delay;

    public PingServerTask(double delay = 0)
    {
        _delay = delay;
    }

    protected override async Task Start()
    {
        await Task.Delay(TimeSpan.FromSeconds(_delay));
        await Task.Run(RunMineStat);
        ServerInfo.IsReady = true;
        
        ServerInfo.IsOnline = _mineStat.ServerUp;
        if (ServerInfo.IsOnline)
        {
            ServerInfo.Latency = _mineStat.Latency;
            ServerInfo.Version = _mineStat.Version;
            ServerInfo.CurrentPlayers = _mineStat.CurrentPlayersInt;
            ServerInfo.MaximumPlayers = _mineStat.MaximumPlayersInt;
            ServerInfo.PlayerList = _mineStat.PlayerList;
        }
    }

    private void RunMineStat()
    {
        _mineStat = new MineStat("minecraft.bronuh.ru", 25565);
    }
    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        if(IsCancelled)
            return null;

        var ping = new PingServerTask(15d);
        return [ping];
    }
}