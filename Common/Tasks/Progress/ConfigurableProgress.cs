namespace Common.Tasks.Progress;

public class ConfigurableProgress : IProgress
{
    public bool IsProgressBarVisible { get; }
    public double Progress => _progressTracker();
    public string ProgressString => _progressStringProvider();
    
    private readonly Func<double> _progressTracker;
    private readonly Func<string> _progressStringProvider;
    
    // ReSharper disable once ConvertToPrimaryConstructor
    public ConfigurableProgress(Func<double> progressTracker, Func<string> progressStringProvider, bool isProgressBarVisible)
    {
        _progressTracker = progressTracker ?? throw new ArgumentNullException(nameof(progressTracker));
        _progressStringProvider = progressStringProvider ?? throw new ArgumentNullException(nameof(progressStringProvider));
        IsProgressBarVisible = isProgressBarVisible;
    }

}