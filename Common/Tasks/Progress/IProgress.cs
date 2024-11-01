namespace Common.Tasks.Progress;

public interface IProgress
{
    /// <summary>
    /// Determines if this progress type can provide Progress values other than just 0 and 1.
    /// </summary>
    bool IsProgressBarVisible { get; }
    
    /// <summary>
    ///  Progress value for graphical representation. Value must be between 0 and 1.
    /// </summary>
    double Progress { get; }
    
    /// <summary>
    /// Short info about progress.
    /// </summary>
    string ProgressString { get; }
}