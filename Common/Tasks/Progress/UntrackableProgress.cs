#region

using JetBrains.Annotations;

#endregion

namespace Common.Tasks.Progress;

public class UntrackableProgress : IProgress
{
    [PublicAPI]
    public string Message { get; set; }
    
    public bool IsProgressBarVisible => false;
    
    [PublicAPI]
    public double Progress { get; set; }
    public string ProgressString => Message;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public UntrackableProgress(string message)
    {
        Message = message;
    }
}