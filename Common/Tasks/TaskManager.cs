using JetBrains.Annotations;

namespace Common.Tasks;

public class TaskManager
{
    private bool _isWorking;
    
    [PublicAPI]
    public List<LauncherTask> PendingTasks { get; private set; } = new ();
    
    [PublicAPI]
    public IEnumerable<LauncherTask> ReadyToRunTasks => PendingTasks.Where(task => task.CanRun 
        && task.State is not (TaskState.Finished or TaskState.Finalized) and not TaskState.Started);
    
    [PublicAPI]
    public List<LauncherTask> UnfinishedTasks { get; private set; } = new ();
    
    [PublicAPI]
    public List<LauncherTask> RunningTasks { get; private set; } = new ();

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly Scheduler _scheduler;
    private readonly uint _workersCount;

    private uint WorkersCount
    {
        get => _workersCount;
        init => _workersCount = Math.Max(0, value);
    }

    public TaskManager(Scheduler scheduler, uint workersCount = 5)
    {
        _scheduler = scheduler;
        WorkersCount = workersCount;
        Start();
    }
    
    [PublicAPI]
    public void AddTask(LauncherTask pendingTask)
    {
        PendingTasks.Add(pendingTask);
        UnfinishedTasks.Add(pendingTask);
    }
    
    [PublicAPI]
    public void AddTasks(IEnumerable<LauncherTask> pendingTasks)
    {
        foreach (var task in pendingTasks)
        {
            AddTask(task);
        }
    }

    [PublicAPI]
    public void Start()
    {
        if (!_isWorking)
            return;
            
        _isWorking = true;
        _scheduler.RunCoroutine(RunTasksCoroutine());
    }

    [PublicAPI]
    public void Stop()
    {
        _isWorking = false;
    }

    private IEnumerator<ITiming> RunTasksCoroutine()
    {
        while (_isWorking)
        {
            if (PendingTasks.Any() || RunningTasks.Any())
            {
                if (RunningTasks.Count(t => t.TakingSlot) < WorkersCount && ReadyToRunTasks.Any())
                {
                    var runningTask = ReadyToRunTasks.First();
                    PendingTasks.Remove(runningTask);
                    RunningTasks.Add(runningTask);
                    _ = runningTask.Run();
                }
                
                foreach (var finishedTask in RunningTasks.Where(t => t.State is TaskState.Finished))
                {
                    var newTasks = finishedTask.OnTaskFinished();
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

                    finishedTask.State = TaskState.Finalized;
                }
            
                UnfinishedTasks.RemoveAll(task => task.IsBranchFinished);

                RunningTasks.RemoveAll(t => t.IsTaskFinished);
            }
            yield return Wait.ForNextTick();
        }
    }
}