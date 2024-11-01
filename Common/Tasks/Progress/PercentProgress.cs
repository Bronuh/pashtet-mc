namespace Common.Tasks.Progress;

public class PercentProgress : IProgress
{
    public double Percentage
    {
        get => _percentage;
        set => _percentage = Math.Clamp(value, 0, 100);
    }
    
    public bool IsProgressBarVisible => true;
    public double Progress => Percentage / 100d;
    public string ProgressString => $"{Percentage}%";

    
    private double _percentage;
}