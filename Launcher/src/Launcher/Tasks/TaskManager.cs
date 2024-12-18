﻿#region

using System.Linq;
using KludgeBox.Events.Global;
using KludgeBox.Scheduling;
using PatchApi.Events;

#endregion

namespace Launcher.Tasks;

public class TaskManager
{
    public bool IsWorking { get; private set; } = false;
    public List<LauncherTask> PendingTasks { get; private set; } = new ();
    public IEnumerable<LauncherTask> ReadyToRunTasks => PendingTasks.Where(task => task.CanRun 
        && task.State is not (TaskState.Finished or TaskState.Finalized) and not TaskState.Started);
    
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
        if (EventBus.PublishIsCancelled(new TaskAboutToAddEvent(pendingTask)))
            return;
        
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
                if (RunningTasks.Count(t => t.TakingSlot) < WorkersCount && ReadyToRunTasks.Any())
                {
                    var runningTask = ReadyToRunTasks.First();
                    PendingTasks.Remove(runningTask);

                    RunningTasks.Add(runningTask);
                    if (runningTask.SkipConditions.Any(condition => condition?.Invoke() ?? true))
                    {
                        runningTask.State = TaskState.Finished;
                    }
                    else
                    {
                        _ = runningTask.Run();
                    }
                }
                
                foreach (var finishedTask in RunningTasks.Where(t => t.State is TaskState.Finished))
                {
                    var newTasks = finishedTask.OnTaskFinished();
                    var evt = new TaskFinishedEvent(finishedTask, newTasks);
                    if (!EventBus.PublishIsCancelled(evt))
                    {
                        if (evt.ChildTasks is not null)
                        {
                            foreach (var newTask in evt.ChildTasks)
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
                    
                    finishedTask.State = TaskState.Finalized;
                }
            
                UnfinishedTasks.RemoveAll(task => task.IsBranchFinished);

                RunningTasks.RemoveAll(t => t.IsTaskFinished);
            }
            yield return Wait.ForNextTick();
        }
    }
}