namespace KludgeBox.Core;

public abstract class AutoSingleton<TSingleton> where TSingleton : AutoSingleton<TSingleton>, new()
{
    public static TSingleton Instance { get; private set; }
    
    static AutoSingleton()
    {
        Instance = new TSingleton();
    }
    
    public static void Reset(TSingleton instance)
    {
        Instance = instance;
    }
}