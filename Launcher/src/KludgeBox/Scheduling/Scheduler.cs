using System.Diagnostics;
using KludgeBox.Godot.Extensions;

namespace KludgeBox.Scheduling;


/// <summary>
/// Manages and executes timed tasks and coroutines.
/// </summary>
public sealed class Scheduler
{
    /// <summary>
    /// Gets the total time elapsed since the first update.
    /// </summary>
    public double TotalTime { get; private set; } = 0;
    
    /// <summary>
    /// Gets the total number of updates performed by the scheduler.
    /// </summary>
    public long TotalTicks { get; private set; } = 0;
    
    /// <summary>
    /// Gets the currently running task within the <see cref="Update"/> call, if any.
    /// </summary>
    public IPendingTask CurrentTask { get; private set; } = null;
    
    private List<IPendingTask> _addBuffer = new();
    private List<IPendingTask> _pendingTasks = new();
    private List<IPendingTask> _nextPendingTasks = new();

    private Stopwatch _stopwatch = new();
    
    /// <summary>
    /// Updates the scheduler and processes tasks. Should be called periodically (e.g., from _Process or _PhysicsProcess).
    /// </summary>
    /// <param name="delta">Time passed since the last call. If not specified, the internal stopwatch result will be used.</param>
    public void Update(double delta = -1)
    {
        var actualDelta = delta < 0 ? _stopwatch.Elapsed.TotalSeconds : delta;
        _stopwatch.Reset();
        _stopwatch.Start();
        
        TotalTime += actualDelta;
        TotalTicks++;

        ProcessTasks();
    }

    /// <summary>
    /// Starts a coroutine task.
    /// </summary>
    /// <param name="coroutine">The coroutine to run.</param>
    /// <param name="runCondition">Optional condition for the first run. If not provided, it will run in the next <see cref="Update"/>.</param>
    /// <param name="parent">Optional Godot object to attach the task to.</param>
    /// <returns>A <see cref="CoroutineTask"/> representing the running coroutine.</returns>
    public CoroutineTask RunCoroutine(IEnumerator<ITiming> coroutine, ConditionDelegate runCondition = null, GodotObject parent = null)
    {
        var task = new CoroutineTask(coroutine);
        if(runCondition is not null)
        {
            task.RunCondition = runCondition;
        }

        if (parent.IsValid())
        {
            task.AttachTo(parent);
        }
        AddTask(task);
        
        return task;
    }

    /// <summary>
    /// Schedules an action to run after a specified delay in seconds.
    /// </summary>
    /// <param name="delay">The time to wait before running the action.</param>
    /// <param name="action">The action to run.</param>
    /// <returns>A <see cref="SecondsDelayedTask"/> representing the delayed task.</returns>
    public SecondsDelayedTask DelayForSeconds(double delay, Action action)
    {
        var task = new SecondsDelayedTask(TotalTime + delay, action);
        AddTask(task);
        
        return task;
    }

    /// <summary>
    /// Schedules an action to run after a specified number of updates (ticks).
    /// </summary>
    /// <param name="delay">The number of updates to wait before running the action.</param>
    /// <param name="action">The action to run.</param>
    public TicksDelayedTask DelayForTicks(long delay, Action action)
    {
        var task = new TicksDelayedTask(TotalTicks + delay, action);
        AddTask(task);
        
        return task;
    }

    /// <summary>
    /// Schedules an action to run periodically with a specified interval in seconds.
    /// </summary>
    /// <param name="interval">The interval between each execution in seconds.</param>
    /// <param name="action">The action to run.</param>
    /// <param name="runImmediately">If <c>true</c>, the action will run immediately without waiting for the first interval.</param>
    /// <returns>A <see cref="PeriodicSecondsTask"/> representing the periodic task.</returns>
    public PeriodicSecondsTask PeriodicInSeconds(double interval, Action action, bool runImmediately = false)
    {
        var task = new PeriodicSecondsTask(interval, runImmediately ? TotalTime : TotalTime + interval, action);
        AddTask(task);
        
        return task;
    }

    /// <summary>
    /// Schedules an action to run periodically with a specified interval in ticks (updates).
    /// </summary>
    /// <param name="interval">The number of ticks between each execution.</param>
    /// <param name="action">The action to run.</param>
    /// <param name="runImmediately">If <c>true</c>, the action will run immediately without waiting for the first interval.</param>
    /// <returns>A <see cref="PeriodicTicksTask"/> representing the periodic task.</returns>
    public PeriodicTicksTask PeriodicInTicks(long interval, Action action, bool runImmediately = false)
    {
        var task = new PeriodicTicksTask(interval, runImmediately ? TotalTicks : TotalTicks + interval, action);
        AddTask(task);
        
        return task;
    }

    /// <summary>
    /// Schedules an action to run as soon as the next step of a target task is reached.
    /// </summary>
    /// <remarks>
    /// This task is not precise and may run either on the same update as the target task or the next.
    /// </remarks>
    /// <param name="targetTask">The task whose next step should be awaited.</param>
    /// <param name="action">The action to run after the target task's next step.</param>
    /// <returns>A <see cref="ManualTask"/> that will run after the next step of the target task.</returns>
    public ManualTask WaitForNextStep(IPendingTask targetTask, Delegate action)
    {
        var currentRuns = targetTask.State.RunCount;
        ConditionDelegate condition = (state) => targetTask.State.RunCount >= currentRuns;
        var task = new ManualTask(condition, action);
        AddTask(task);
        
        return task;
    }

    /// <summary>
    /// Schedules an action to run after a target task is completed.
    /// </summary>
    /// <remarks>
    /// This task is not precise and may run either on the same update as the target task or the next.
    /// </remarks>
    /// <param name="targetTask">The task whose completion should be awaited.</param>
    /// <param name="action">The action to run after the target task is completed.</param>
    /// <returns>A <see cref="ManualTask"/> that will run after the target task is completed.</returns>
    public ManualTask WaitForCompletion(IPendingTask targetTask, Delegate action)
    {
        ConditionDelegate condition = (state) => targetTask.State.IsCompleted;
        var task = new ManualTask(condition, action);
        AddTask(task);
        
        return task;
    }
    
    
    private SchedulerState GetState()
    {
        return new(TotalTicks, TotalTime, CurrentTask);
    }
    
    private void ProcessTasks()
    {
        var nextTasks = _nextPendingTasks;
        nextTasks.AddRange(_addBuffer);
        _addBuffer.Clear();

        foreach (var task in _pendingTasks)
        {
            CurrentTask = task;
            try
            {
                // Drop all tasks that should not run anymore
                if (!task.IsValid || task.State.IsCanceled || task.State.IsCompleted)
                {
                    task.State.IsCompleted = true;
                    continue;
                }
                
                // Pass paused tasks to next update
                if(task.State.IsPaused)
                {
                    nextTasks.Add(task);
                    continue;
                }
                
                // Pass tasks that are not ready to next update
                var state = GetState();
                if(!task.RunCondition(state))
                {
                    nextTasks.Add(task);
                    continue;
                }

                var request = task.Run(state);
                task.State.RunCount++;
                var newTask = request?.GetNextStep(state);

                if (newTask is not null)
                {
                    nextTasks.Add(newTask);
                }
                else
                {
                    task.State.IsCompleted = true;
                }
            }
            catch (Exception e)
            {
                Log.Warning($"Exception thrown while processing task {task}: {e}");
                Log.Error();
            }
        }
        
        CurrentTask = null;
        
        _pendingTasks.Clear();
        _nextPendingTasks = _pendingTasks;
        _pendingTasks = nextTasks;
    }
    
    private void AddTask(IPendingTask task)
    {
        _addBuffer.Add(task);
    }
}

/// <summary>
/// Represents the state of the scheduler at a specific moment in time.
/// </summary>
public readonly record struct SchedulerState(long TotalTicks, double TotalTime, IPendingTask CurrentTask);


/// <summary>
/// Provides methods to create task delay timings for scheduling.
/// </summary>
public static class Wait
{
    /// <summary>
    /// Creates a delay timing for a task that should wait for the specified number of seconds.
    /// </summary>
    /// <param name="seconds">The delay duration in seconds.</param>
    /// <returns>A <see cref="SecondsDelayTiming"/> that delays execution for the specified seconds.</returns>
    public static SecondsDelayTiming Seconds(double seconds) => new(seconds);
    
    /// <summary>
    /// Creates a delay timing for a task that should wait for the specified number of ticks (updates).
    /// </summary>
    /// <param name="ticks">The delay duration in ticks.</param>
    /// <returns>A <see cref="TicksDelayTiming"/> that delays execution for the specified ticks.</returns>
    public static TicksDelayTiming Ticks(long ticks) => new(ticks);
    
    /// <summary>
    /// Creates a timing that allows the task to wait until the next scheduler tick (update).
    /// </summary>
    /// <returns>A <see cref="NextTickTiming"/> that runs on the next tick.</returns>
    public static NextTickTiming ForNextTick() => new();
    
    /// <summary>
    /// Creates a timing that allows the task to wait until the next step of the specified task is reached.
    /// </summary>
    /// <param name="targetTask">The task whose next step is awaited.</param>
    /// <returns>A <see cref="NextTaskStepTiming"/> that runs after the next step of the <paramref name="targetTask"/>.</returns>
    public static NextTaskStepTiming ForTaskNextStep(IPendingTask targetTask) => new NextTaskStepTiming(targetTask);
    
    /// <summary>
    /// Creates a timing that allows the task to wait until the specified task has completed.
    /// </summary>
    /// <param name="targetTask">The task whose completion is awaited.</param>
    /// <returns>A <see cref="TaskCompletionTiming"/> that runs after the <paramref name="targetTask"/> completes.</returns>
    public static TaskCompletionTiming ForTaskCompletion(IPendingTask targetTask) => new TaskCompletionTiming(targetTask);
    
    /// <summary>
    /// Creates a special timing that indicates the task should exit and not continue.
    /// </summary>
    /// <returns>An <see cref="ITiming"/> object that causes the task to exit.</returns>
    public static ITiming Exit() => null;
}




/// <summary>
/// Represents a pending task that can be controlled by the scheduler.
/// </summary>
public interface IPendingTask
{
    /// <summary>
    /// Gets the state of the task.
    /// </summary>
    TaskState State { get; }
    
    /// <summary>
    /// Gets a value indicating whether the task is valid and can be run.
    /// </summary>
    bool IsValid { get; }
    
    /// <summary>
    /// Gets or sets the condition that determines when the task should run.
    /// </summary>
    ConditionDelegate RunCondition { get; set; }
    
    /// <summary>
    /// Runs the task.
    /// </summary>
    /// <param name="state">The current state of the scheduler.</param>
    /// <returns>An <see cref="ITiming"/> object representing the next step, or <c>null</c> if the task is completed.</returns>
    ITiming Run(SchedulerState state);

    /// <summary>
    /// Cancels the task, preventing further execution.
    /// </summary>
    public void Cancel()
    {
        State.IsCanceled = true;
    }
}

/// <summary>
/// Represents a task that runs after a specified delay in seconds.
/// </summary>
public sealed class SecondsDelayedTask : IPendingTask
{
    public TaskState State { get; } = new();
    public bool IsValid { get; } = true;
    public ConditionDelegate RunCondition { get; set; }
    public Action RunAction { get; private set; }
    
    
    public SecondsDelayedTask(double targetTime, Action action)
    {
        RunCondition = state => state.TotalTime >= targetTime;
        RunAction = action;
    }
    public ITiming Run(SchedulerState state)
    {
        RunAction.Invoke();
        return null;
    }
}

/// <summary>
/// Represents a task that runs after a specified number of ticks (updates).
/// </summary>
public sealed class TicksDelayedTask : IPendingTask
{
    public TaskState State { get; } = new();
    public bool IsValid { get; } = true;
    public ConditionDelegate RunCondition { get; set; }
    public Action RunAction { get; private set; }
    
    
    public TicksDelayedTask(long targetTick, Action action)
    {
        RunCondition = state => state.TotalTicks >= targetTick;
        RunAction = action;
    }
    public ITiming Run(SchedulerState state)
    {
        RunAction.Invoke();
        return null;
    }
}

/// <summary>
/// Represents a task that runs periodically with a specified interval in seconds.
/// </summary>
public sealed class PeriodicSecondsTask : IPendingTask
{
    public TaskState State { get; } = new();
    public bool IsValid { get; } = true;
    public double Interval { get; private set; }
    public ConditionDelegate RunCondition { get; set; }
    public Action RunAction { get; private set; }
    
    
    public PeriodicSecondsTask(double interval, double targetTime, Action action)
    {
        RunCondition = state => state.TotalTime >= targetTime;
        RunAction = action;
        Interval = interval;
    }
    
    public ITiming Run(SchedulerState state)
    {
        RunAction.Invoke();
        return Wait.Seconds(Interval);
    }
}

/// <summary>
/// Represents a task that runs periodically with a specified interval in ticks (updates).
/// </summary>
public sealed class PeriodicTicksTask : IPendingTask
{
    public TaskState State { get; } = new();
    public bool IsValid { get; } = true;
    public long Interval { get; private set; }
    public ConditionDelegate RunCondition { get; set; }
    public Action RunAction { get; private set; }
    
    
    public PeriodicTicksTask(long interval, double targetTime, Action action)
    {
        RunCondition = state => state.TotalTime >= targetTime;
        RunAction = action;
        Interval = interval;
    }
    
    public ITiming Run(SchedulerState state)
    {
        RunAction.Invoke();
        return Wait.Ticks(Interval);
    }
}

/// <summary>
/// Represents a manual task that runs based on a custom condition and action.
/// </summary>
public sealed record ManualTask(ConditionDelegate condition, Delegate runAction) : IPendingTask
{
    public TaskState State { get; } = new();
    public bool IsValid => RunCondition is not null && RunAction is not null;
    public ConditionDelegate RunCondition { get; set; } = condition;
    public Delegate RunAction { get; private set; } = runAction;
    public ITiming Run(SchedulerState state)
    {
        var result = RunAction?.DynamicInvoke(state);
        
        if (result is ITiming timing)
        {
            return timing;
        }

        return null;
    }
}

/// <summary>
/// Represents a task that runs a coroutine.
/// </summary>
/// <param name="Coroutine">The coroutine to be run by the task.</param>
public sealed class CoroutineTask(IEnumerator<ITiming> Coroutine) : IPendingTask
{
    public TaskState State { get; } = new();
    
    public bool IsAtatched { get; private set; }
    public GodotObject Parent { get; private set; }
    public bool IsValid => !(IsAtatched && !GodotObject.IsInstanceValid(Parent));
    public ConditionDelegate RunCondition { get; set; } = _ => true;

    public ITiming Run(SchedulerState state)
    {
        if (Coroutine.MoveNext())
        {
            var nextStep = Coroutine.Current;
            return nextStep;
        }

        return null;
    }

    public void AttachTo(GodotObject godotObject)
    {
        IsAtatched = true;
        Parent = godotObject;
    }

    public void Detach()
    {
        IsAtatched = false;
        Parent = null;
    }
}



/// <summary>
/// Represents the state of a task, including its completion and cancellation status.
/// </summary>
public sealed class TaskState
{
    public bool IsCompleted;
    public bool IsCanceled;
    public int RunCount;
    public bool IsPaused;
}



/// <summary>
/// Delegate for defining a condition under which a task should run.
/// </summary>
/// <param name="state">The current state of the scheduler.</param>
/// <returns><c>true</c> if the task should run; otherwise, <c>false</c>.</returns>
public delegate bool ConditionDelegate(SchedulerState state);



/// <summary>
/// Represents a timing step in the scheduler for controlling task execution.
/// </summary>
public interface ITiming
{
    IPendingTask GetNextStep(SchedulerState state);
}



public record SecondsDelayTiming(double time) : ITiming
{
    public IPendingTask GetNextStep(SchedulerState state)
    {
        var task = state.CurrentTask;
        var targetTime = state.TotalTime + time;
        task.RunCondition = schedulerState => schedulerState.TotalTime >= targetTime;
        
        return task;
    }
}

public record TicksDelayTiming(long ticks) : ITiming
{
    public IPendingTask GetNextStep(SchedulerState state)
    {
        var task = state.CurrentTask;
        var targetTick = state.TotalTicks + ticks;
        task.RunCondition = schedulerState => schedulerState.TotalTime >= targetTick;
        
        return task;
    }
}

public record NextTickTiming : ITiming
{
    public IPendingTask GetNextStep(SchedulerState state)
    {
        var task = state.CurrentTask;
        task.RunCondition = _ => true;
        return task;
    }
}

public record NextTaskStepTiming(IPendingTask targetTask) : ITiming
{
    public IPendingTask GetNextStep(SchedulerState state)
    {
        var task = state.CurrentTask;
        var monitoringTask = targetTask;
        var targetTick = monitoringTask.State.RunCount + 1;
        task.RunCondition = schedulerState => monitoringTask.State.RunCount >= targetTick;
        
        return task;
    }
}

public record TaskCompletionTiming(IPendingTask targetTask) : ITiming
{
    public IPendingTask GetNextStep(SchedulerState state)
    {
        var task = state.CurrentTask;
        var monitoringTask = targetTask;
        task.RunCondition = schedulerState => monitoringTask.State.IsCompleted;
        
        return task;
    }
}