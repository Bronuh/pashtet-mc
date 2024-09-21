using System.Threading.Tasks;

namespace Tasks.Implementations;

public class FinishPreparationsTask : LauncherTask
{
    public override string Name { get; } = "Завершение подготовки";
    protected override async Task Start()
    {
        Main.Instance.RunButton.Disabled = false;
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return null;
    }
}