using System.Linq;
using KludgeBox.Scheduling;
using Tasks;
using TaskState = Tasks.TaskState;

public class TaskManager
{
    public bool IsWorking { get; private set; } = false;
    public List<LauncherTask> PendingTasks { get; private set; } = new ();
    public IEnumerable<LauncherTask> ReadyToRunTasks => PendingTasks.Where(task => task.CanRun && task.State is not TaskState.Finished and not TaskState.Started);
    
    public List<LauncherTask> UnfinishedTasks { get; private set; } = new ();
    
    public List<LauncherTask> RunningTasks { get; private set; } = new List<LauncherTask>();

    private Scheduler _scheduler;
    private uint _workersCount;

    private uint WorkersCount
    {
        get => _workersCount;
        set => _workersCount = (uint)Mathf.Max(0, value);
    }

    public TaskManager(Scheduler scheduler, uint workersCount = 5)
    {
        _scheduler = scheduler;
        WorkersCount = workersCount;
        scheduler.RunCoroutine(RunTasksCorutine());
    }

    public void AddTask(LauncherTask pendingTask)
    {
        PendingTasks.Add(pendingTask);
        UnfinishedTasks.Add(pendingTask);
    }

    public void AddTasks(IEnumerable<LauncherTask> pendingTasks)
    {
        foreach (var task in pendingTasks)
        {
            AddTask(task);
        }
    }

    private IEnumerator<ITiming> RunTasksCorutine()
    {
        while (true)
        {
            if (PendingTasks.Any() || RunningTasks.Any())
            {
                if (RunningTasks.Count < WorkersCount && ReadyToRunTasks.Any())
                {
                    var runningTask = ReadyToRunTasks.First();
                    PendingTasks.Remove(runningTask);
                    RunningTasks.Add(runningTask);
                    _ = runningTask.Run();
                }
            
                foreach (var finishedTask in RunningTasks.Where(t => t.State is TaskState.Finished))
                {
                    var newTasks = finishedTask.GetNextTasks();
                    if (newTasks is not null)
                    {
                        foreach (var newTask in newTasks)
                        {
                            if(newTask is not null)
                            {
                                AddTask(newTask);
                                newTask.ParentTask = finishedTask;
                                finishedTask.ChildrenTasks.Add(newTask);
                            }
                        }
                    }
                }

                UnfinishedTasks.RemoveAll(task => task.IsBranchFinished);

                RunningTasks.RemoveAll(t => t.State == TaskState.Finished);
            }
            yield return Wait.ForNextTick();
        }
    }
}