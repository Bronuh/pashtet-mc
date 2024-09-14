using System;
using System.IO;

public abstract class UpdateChecker
{
    public Action Action;

    public abstract VersionInfo Check();
    public abstract void Update();

    
}