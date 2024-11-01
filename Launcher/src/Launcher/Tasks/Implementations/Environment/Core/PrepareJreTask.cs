﻿using System.IO;
using System.Threading.Tasks;
using BronuhMcBackend.Utils;
using Launcher;

namespace Tasks.Implementations;

public class PrepareJreTask : LauncherTask
{
    private LauncherTask _nextTask;
    public override string Name { get; } = "Подготовка Java";
    
    protected override async Task Start()
    {
        if (File.Exists(Paths.JreExecutablePath.AsAbsolute()))
        {
            return;
        }

        if (File.Exists(Paths.JreFileName.AsAbsolute()))
        {
            _nextTask = new UnpackJreTask();
            return;
        }

        _nextTask = new DownloadJreTask();
    }

    public override IEnumerable<LauncherTask> OnTaskFinished()
    {
        return [_nextTask];
    }
}