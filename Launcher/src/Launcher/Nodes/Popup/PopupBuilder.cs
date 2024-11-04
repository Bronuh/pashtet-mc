using System.Threading.Tasks;

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

    /// <summary>
    /// This Popup will pause main scheduler when shown, so other tasks will not be processed until this popup is closed.
    /// </summary>
    /// <param name="pause"></param>
    /// <returns></returns>
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
    
    public PopupBuilder WithButton(string title, Action action = null)
    {
        var button = new ButtonRequest(title, WrapAction(action));
        _request.Buttons.Add(button);
        return this;
    }
    
    public PopupBuilder WithCancelButton(Action additionalAction = null)
    {
        _cancelButtonRequired = true;
        _onCancel = additionalAction;
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
        _enqueued = true;
        
        if (_cancelButtonRequired)
            WithButton("Отмена", _onCancel);
        
        _popup.QueuePopup(_request);
    }

    public Task EnqueueAndWaitAsync()
    {
        if (_enqueued)
            throw new InvalidOperationException("Cannot enqueue more than once.");
        
        var tcs = new TaskCompletionSource();
        _request.Closed += (_) => tcs.SetResult();
        Enqueue();
        
        return tcs.Task;
    }

    private Action WrapAction(Action action)
    {
        return () =>
        {
            action?.Invoke();
            _popup.HidePopup();
        };
    }
}