namespace Common.Logging;

public sealed class ConsoleLogger : ILogger
{
    public void Debug(object message = null)
    {
        Console.WriteLine($"[DEBUG]: {message}");
    }

    public void Info(object message = null)
    {
        Console.WriteLine($"[INFO]: {message}");
    }

    public void Warn(object message = null)
    {
        Console.WriteLine($"[WARN]: {message}");
    }

    public void Error(object message = null)
    {
        Console.WriteLine($"[ERROR]: {message}");
    }
}