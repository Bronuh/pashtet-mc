#region

using Common.Logging;
using Common.Tasks.Progress;

#endregion

namespace Common.Tasks;

public enum TaskState
{
    Pending,
    Started,
    Finished,
    
    /// <summary>
    /// OnTaskFinished was called
    /// </summary>
    Finalized
}
public abstract class LauncherTask
{
    private TaskState _state;
    private readonly ILogger _log = DefaultServices.Logger;

    /// <summary>
    /// Current task state
    /// </summary>
    public TaskState State
    {
        get => _state;
        set
        {
            _state = value;
        }
    }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public bool IsCancelled { get; set; }
    public static bool DoThrow { get; set; } = false;
    
    public bool IsBranchFinished => IsTaskFinalized && ChildrenTasks.All(task => task.IsBranchFinished);
    public bool IsTaskFinished => State is TaskState.Finished or TaskState.Finalized;
    public bool IsTaskFinalized => State is TaskState.Finalized;
    
    public bool IsVisible { get; set; } = true;
    public bool TakingSlot { get; set; } = true;

    public virtual bool CanRun => (!RequiredTasks.Any() || RequiredTasks.All(x => x.IsBranchFinished)) && (!AdditionalConditions.Any() || AdditionalConditions.All(x => x?.Invoke() ?? true));
    
    public List<LauncherTask> RequiredTasks { get; private set; } = new ();
    public List<Func<bool>> AdditionalConditions { get; private set; } = new ();
    public LauncherTask ParentTask { get; set; }
    public List<LauncherTask> ChildrenTasks { get; private set; } = new ();

    public abstract string Name { get; }

    public IProgress Progress { get; protected set; } = new UntrackableProgress("");
        

    public async Task Run()
    {
        State = TaskState.Started;
        StartTime = DateTime.Now;
        try
        {
            _log.Info($"Выполняется задача: {Name}");
            await Start();
        }
        catch (Exception e)
        {
            if (DoThrow)
            {
                throw;
            }
            else
            {
                _log.Error($"Ошибка при выполнении задачи:: {Name} {e.Message}\n{e}");
            }
        }
        _log.Info($"Завершена задача: {Name}");
        EndTime = DateTime.Now;
        State = TaskState.Finished;
    }

    public string GetRunningTime()
    {
        // Calculate the time difference
        TimeSpan elapsed = State switch
        {
            TaskState.Pending => TimeSpan.Zero,
            TaskState.Started => DateTime.Now - StartTime,
            TaskState.Finished => EndTime - StartTime,
            TaskState.Finalized => EndTime - StartTime,
            _ => throw new ArgumentOutOfRangeException()
        };

        // Format the output based on the elapsed time
        if (elapsed.TotalSeconds < 1)
        {
            // Show only milliseconds if less than one second
            return $"{elapsed.Milliseconds}ms";
        }
        else if (elapsed.TotalSeconds < 60)
        {
            // Show seconds and milliseconds if less than a minute
            return $"{(int)elapsed.TotalSeconds}s{elapsed.Milliseconds}ms";
        }
        else if (elapsed.TotalMinutes < 60)
        {
            // Show minutes, seconds, and milliseconds if less than an hour but more than a minute
            return $"{(int)elapsed.TotalMinutes}m{elapsed.Seconds}s{elapsed.Milliseconds}ms";
        }
        else if (elapsed.TotalHours < 24)
        {
            // Show hours, minutes, seconds, and milliseconds if less than a day
            return $"{(int)elapsed.TotalHours}h{elapsed.Minutes}m{elapsed.Seconds}s{elapsed.Milliseconds}ms";
        }
        else
        {
            // Show days, hours, minutes, seconds, and milliseconds if more than a day
            return $"{(int)elapsed.TotalDays}d{elapsed.Hours}h{elapsed.Minutes}m{elapsed.Seconds}s{elapsed.Milliseconds}ms";
        }
    }
    
    /// <summary>
    /// Specifies additional run condition.
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public LauncherTask WithCondition(Func<bool> condition)
    {
        AdditionalConditions.Add(condition);
        return this;
    }

    /// <summary>
    /// This task will run only after all the specified tasks and their subtasks will be finished.
    /// </summary>
    /// <param name="tasks"></param>
    /// <returns></returns>
    public LauncherTask AfterTasks(params LauncherTask[] tasks)
    {
        RequiredTasks.AddRange(tasks);
        return this;
    }
    
    
    /// <summary>
    /// Runs finishing logic
    /// </summary>
    /// <returns>Next tasks to perform</returns>
    public abstract IEnumerable<LauncherTask> OnTaskFinished();

    /// <summary>
    /// Counts a current task generation. Every task returned from <see cref="OnTaskFinished"/> has incremented task generation.
    /// </summary>
    /// <returns></returns>
    public int GetTaskGeneration()
    {
        if (ParentTask is null)
            return 0;
        
        return ParentTask.GetTaskGeneration() + 1;
    }
    
    protected abstract Task Start();
    
    private string GetNameSafe()
    {
        try
        {
            return Name;
        }
        catch (Exception e)
        {
            return $"NAME ERRORED ({e.Message})";
        }
    }
}