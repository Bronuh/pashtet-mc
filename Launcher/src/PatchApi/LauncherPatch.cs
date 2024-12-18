﻿namespace PatchApi;

public abstract class LauncherPatch
{
    /// <summary>
    /// The bigger this value, the earlier this patch will be applied. Use values less than 0 to run patches later. <br /><br />
    /// <b>Default is 0</b>.
    /// </summary>
    public long Priority { get; protected set; } = 0;
    public bool WasRun { get; private set; } = false;
    
    protected abstract void Run();
    public virtual bool CanRun() => true;

    internal void ProcessAndRun()
    {
        Run();
        WasRun = true;
    }
}