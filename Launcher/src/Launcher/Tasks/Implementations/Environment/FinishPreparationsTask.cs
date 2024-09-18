﻿using System.Threading.Tasks;

namespace Tasks.Implementations;

public class FinishPreparationsTask : LauncherTask
{
    public override string Name { get; } = "Завершение подготовки";
    protected override async Task Start()
    {
        // do nothing
    }

    public override IEnumerable<LauncherTask> GetNextTasks()
    {
        return null;
    }
}