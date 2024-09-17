#region

using System.Reflection;
using KludgeBox.Events.EventTypes;

#endregion

namespace KludgeBox.Events;

internal sealed class EventHub
{
    public Type EventHubType { get; private set;}
    public bool IsActive { get; private set; } = true;

    private List<IListener>[] _listenersByPriority;
    private List<ListenerToken> _markedForDeletion = new();
    private List<(object info, ListenerPriority priority)> _queuedForSubscription = new();
    

    private bool _isPublishing = false;
    private object _publishingLock = new();
    private object _unsubLock = new();
    private object _subLock = new();
    public EventHub(Type type)
    {
        EventHubType = type;
        var prioritiesCount = Enum.GetValues(typeof(ListenerPriority)).Length;
        _listenersByPriority = new List<IListener>[prioritiesCount];
        for (int i = 0; i < prioritiesCount; i++)
        {
            _listenersByPriority[i] = new();
        }
    }
    
    internal void Publish<T>(T @event) where T : IEvent
    {
        lock (_publishingLock)
        {
            if (@event is not null)
            {
                var handleableEvent = @event as HandleableEvent;
                var isHandleable = handleableEvent is not null;
                
                _isPublishing = true;
                foreach (var priority in _listenersByPriority)
                {
                    foreach (var listener in priority)
                    {
                        if (isHandleable && handleableEvent.IsHandled) break;
                        listener?.Deliver(@event);
                    }
                }
                _isPublishing = false;
                
                foreach (var token in _markedForDeletion)
                {
                    Unsubscribe(token);
                }
                _markedForDeletion.Clear();

                foreach (var (info, priority) in _queuedForSubscription)
                {
                    var infoType = info.GetType().GenericTypeArguments[0];
                    var subscribeMethod = GetType()
                        .GetMethod("Subscribe", BindingFlags.Instance | BindingFlags.NonPublic)
                        ?.MakeGenericMethod([infoType]);

                    subscribeMethod?.Invoke(this, [info, priority]);
                }
                _queuedForSubscription.Clear();
            }
        }
    }

    internal ListenerToken Subscribe<T>(ListenerInfo<T> info, ListenerPriority priority) where T : IEvent
    {
        lock (_subLock)
        {
            if (!_isPublishing)
            {
                var priorityListeners = _listenersByPriority[(int)priority];
        
                var subscription = new Listener<T>(info.Action);
                priorityListeners.Add(subscription);
                var token = new ListenerToken(subscription, this);
                return token;
            }
            else
            {
                _queuedForSubscription.Add((info, priority));
                return new ListenerToken(null, this);
            }
            
        }
    }

    internal void Unsubscribe(ListenerToken token)
    {
        lock (_unsubLock)
        {
            if(!_isPublishing)
            {
                foreach (var listeners in _listenersByPriority)
                {
                    listeners.Remove(token.Listener);
                }
            }
            else
            {
                _markedForDeletion.Add(token);
            }
        }
    }

    public void Reset()
    {
        var prioritiesCount = Enum.GetValues(typeof(ListenerPriority)).Length;
        _listenersByPriority = new List<IListener>[prioritiesCount];
    }

    public void Deactivate()
    {
        IsActive = true;
    }
}