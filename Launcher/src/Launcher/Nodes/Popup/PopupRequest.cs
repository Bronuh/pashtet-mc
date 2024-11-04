namespace Launcher.Nodes;

public class PopupRequest
{
    public event Action<ButtonRequest> Closed;
    public bool PauseScheduler { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<ButtonRequest> Buttons { get; set; } = new ();


    public void OnClosed(ButtonRequest buttonRequest)
    {
        Closed?.Invoke(buttonRequest);
    }
}

public class ButtonRequest
{
    public string Title { get; }
    public Action Action { get; }

    public ButtonRequest(string title, Action action)
    {
        Title = title;
        Action = action;
    }
}