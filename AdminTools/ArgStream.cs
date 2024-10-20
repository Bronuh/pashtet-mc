namespace AdminTools;

public class ArgStream
{
    public int Position { get; set; }
    private string[] _args;
    
    public ArgStream(string[] args)
    {
        _args = args;
    }
    
    public bool CanRead() => Position < _args.Length;

    public bool TryRead(out string value)
    {
        if (CanRead())
        {
            value = _args[Position++];
            return true;
        }
        
        value = null;
        return false;
    }
    
    public string[] GetSourceArray() => _args;
}