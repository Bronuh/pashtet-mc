﻿using System.Linq;
using System.Threading.Tasks;
using KludgeBox.Scheduling;

namespace Tasks;

public enum TaskState
{
    Pending,
    Started,
    Finished
}
public abstract class LauncherTask
{
    /// <summary>
    /// Current task state
    /// </summary>
    public TaskState State { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    public bool IsBranchFinished => State == TaskState.Finished && ChildrenTasks.All(task => task.IsBranchFinished);

    public virtual bool CanRun => (!RequiredTasks.Any() || RequiredTasks.All(x => x.IsBranchFinished)) && (!AdditionalConditions.Any() || AdditionalConditions.All(x => x?.Invoke() ?? true));
    public string RunningTime
    {
        get
        {
            // Calculate the time difference
            TimeSpan elapsed = State switch
            {
                TaskState.Pending => TimeSpan.Zero,
                TaskState.Started => DateTime.Now - StartTime,
                TaskState.Finished => EndTime - StartTime,
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
    }
    
    public List<LauncherTask> RequiredTasks { get; private set; } = new List<LauncherTask>();
    public List<Func<bool>> AdditionalConditions { get; private set; } = new List<Func<bool>>();
    public LauncherTask ParentTask { get; set; }
    
    public List<LauncherTask> ChildrenTasks { get; private set; } = new List<LauncherTask>();
    
    public string Name { get; protected set; }

    /// <summary>
    /// Value in range 0-1, representing progress.
    /// </summary>
    public virtual double Progress => CompletedWorkAmount / TotalWorkAmount;

    public virtual double TotalWorkAmount { get; } = 1;
    public virtual double CompletedWorkAmount { get; } = 1;

    public async Task Run()
    {
        State = TaskState.Started;
        StartTime = DateTime.Now;
        await Start();
        EndTime = DateTime.Now;
        State = TaskState.Finished;
    }

    public LauncherTask WithCondition(Func<bool> condition)
    {
        AdditionalConditions.Add(condition);
        return this;
    }

    public LauncherTask AfterTasks(params LauncherTask[] tasks)
    {
        RequiredTasks.AddRange(tasks);
        return this;
    }
    
    protected abstract Task Start();

    public abstract IEnumerable<LauncherTask> GetNextTasks();

    public int GetTaskGeneration()
    {
        if (ParentTask is null)
            return 0;
        
        return ParentTask.GetTaskGeneration() + 1;
    }
}