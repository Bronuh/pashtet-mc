namespace Launcher.Nodes;

public class PopupBuilder
{
    private Popup _popup;
    private bool _cancelButtonRequired;
    private Action _onCancel;
    private bool _enqueued;
    
    private PopupRequest _request = new PopupRequest();
    public PopupBuilder(Popup popup)
    {
        _popup = popup;
    }

    public PopupBuilder PauseScheduler(bool pause = true)
    {
        _request.PauseScheduler = pause;
        return this;
    }

    public PopupBuilder WithTitle(string title)
    {
        _request.Title = title;
        return this;
    }

    public PopupBuilder WithDescription(string content)
    {
        _request.Description = content;
        return this;
    }
    
    public PopupBuilder WithButton(string title, Action action)
    {
        var button = new ButtonRequest(title, WrapAction(action));
        _request.Buttons.Add(button);
        return this;
    }
    
    public PopupBuilder WithCancelButton(Action additionalAction = null)
    {
        _cancelButtonRequired = true;
        return this;
    }

    public PopupBuilder WithCloseCallback(Action<ButtonRequest> action)
    {
        _request.Closed += action;
        return this;
    }
    
    public void Enqueue()
    {
        if (_enqueued)
            throw new InvalidOperationException("Cannot enqueue more than once.");

        if (_cancelButtonRequired)
            WithButton("Отмена", WrapAction(_onCancel));
        
        _popup.QueuePopup(_request);
    }

    private Action WrapAction(Action action)
    {
        return () =>
        {
            action?.Invoke();
            _popup.Hide();
        };
    }
}