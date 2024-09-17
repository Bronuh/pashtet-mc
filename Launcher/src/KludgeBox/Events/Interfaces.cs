#region

using KludgeBox.Events.EventTypes;

#endregion

namespace KludgeBox.Events;

internal interface IListener
{
    void Deliver(IEvent @event);
}

