namespace Common.Logging;

public interface ILogger
{
    void Debug(object message = null);
    void Info(object message = null);
    void Warn(object message = null);
    void Error(object message = null);
}