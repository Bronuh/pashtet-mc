namespace Common.Tasks.Progress;

public class IntegerCountProgress : IProgress
{
    public int Count { get; set; }
    public int MaxCount { get; set; }
    
    public bool IsProgressBarVisible => true;
    public double Progress => Count / (double)MaxCount;
    public string ProgressString => $"{Count}/{MaxCount}";
}